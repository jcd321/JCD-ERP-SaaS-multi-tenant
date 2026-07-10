using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Adjustments;
using Jcd.Erp.Domain.Inventory.Movements;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Warehouses;
using MediatR;

namespace Jcd.Erp.Application.Adjustments.Commands.CreateInventoryAdjustment;

public class CreateInventoryAdjustmentHandler : IRequestHandler<CreateInventoryAdjustmentCommand, Result<Guid>>
{
    private readonly IInventoryAdjustmentRepository _adjustmentRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IStockLevelRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryAdjustmentHandler(
        IInventoryAdjustmentRepository adjustmentRepository,
        IInventoryMovementRepository movementRepository,
        IStockLevelRepository stockRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _adjustmentRepository = adjustmentRepository;
        _movementRepository = movementRepository;
        _stockRepository = stockRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateInventoryAdjustmentCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse is null)
            return Result.Failure<Guid>("Adjustment.WarehouseNotFound");

        if (!warehouse.IsActive)
            return Result.Failure<Guid>("Adjustment.WarehouseInactive");

        var productIds = request.Lines.Select(l => l.ProductId).Distinct().ToList();
        if (productIds.Count != request.Lines.Count)
            return Result.Failure<Guid>("Adjustment.DuplicateProduct");

        var lineData = new List<(Guid ProductId, decimal QuantityBefore, decimal CountedQuantity)>();

        foreach (var line in request.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product is null)
                return Result.Failure<Guid>("Adjustment.ProductNotFound");

            if (!product.IsActive)
                return Result.Failure<Guid>("Adjustment.ProductInactive");

            var stockLevel = await _stockRepository.GetByProductAndWarehouseAsync(
                line.ProductId,
                request.WarehouseId,
                cancellationToken);

            var quantityBefore = stockLevel?.QuantityOnHand ?? 0;

            if (quantityBefore == line.CountedQuantity)
                return Result.Failure<Guid>("Adjustment.NoChange");

            lineData.Add((line.ProductId, quantityBefore, line.CountedQuantity));
        }

        var adjustmentDate = request.AdjustmentDate ?? DateTime.UtcNow;
        var documentNumber = await _adjustmentRepository.GetNextDocumentNumberAsync(cancellationToken);

        var adjustmentResult = InventoryAdjustment.Create(
            _tenant.TenantId,
            documentNumber,
            request.WarehouseId,
            adjustmentDate,
            request.Reason,
            lineData,
            request.Notes);

        if (adjustmentResult.IsFailure)
            return Result.Failure<Guid>(adjustmentResult.Error);

        var adjustment = adjustmentResult.Value;
        var movementReference = adjustment.DocumentNumber;
        var movementNotes = $"Adjustment {adjustment.DocumentNumber}: {adjustment.Reason}";

        foreach (var line in adjustment.Lines)
        {
            var delta = line.QuantityDelta;
            var movementType = delta > 0 ? InventoryMovementTypes.In : InventoryMovementTypes.Out;
            var quantity = Math.Abs(delta);

            var applyResult = await ApplyMovementAsync(
                line.ProductId,
                request.WarehouseId,
                movementType,
                quantity,
                adjustmentDate,
                movementReference,
                movementNotes,
                cancellationToken);

            if (applyResult.IsFailure)
                return Result.Failure<Guid>(applyResult.Error);
        }

        await _adjustmentRepository.AddAsync(adjustment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(adjustment.Id);
    }

    private async Task<Result> ApplyMovementAsync(
        Guid productId,
        Guid warehouseId,
        string movementType,
        decimal quantity,
        DateTime movementDate,
        string? reference,
        string? notes,
        CancellationToken cancellationToken)
    {
        var stockLevel = await _stockRepository.GetByProductAndWarehouseAsync(
            productId,
            warehouseId,
            cancellationToken);

        if (stockLevel is null)
        {
            if (movementType == InventoryMovementTypes.Out)
                return Result.Failure("Adjustment.InsufficientStock");

            var stockResult = StockLevel.Create(_tenant.TenantId, productId, warehouseId, 0);
            if (stockResult.IsFailure)
                return Result.Failure(stockResult.Error);

            stockLevel = stockResult.Value;
            await _stockRepository.AddAsync(stockLevel, cancellationToken);
        }

        var quantityChange = movementType == InventoryMovementTypes.In ? quantity : -quantity;
        var quantityBefore = stockLevel.QuantityOnHand;
        var applyResult = stockLevel.ApplyQuantityChange(quantityChange);
        if (applyResult.IsFailure)
            return Result.Failure(applyResult.Error);

        var documentNumber = await _movementRepository.GetNextDocumentNumberAsync(cancellationToken);
        var movementResult = InventoryMovement.Create(
            _tenant.TenantId,
            documentNumber,
            productId,
            warehouseId,
            movementType,
            quantity,
            quantityBefore,
            stockLevel.QuantityOnHand,
            movementDate,
            reference,
            notes);

        if (movementResult.IsFailure)
            return Result.Failure(movementResult.Error);

        _stockRepository.Update(stockLevel);
        await _movementRepository.AddAsync(movementResult.Value, cancellationToken);

        return Result.Success();
    }
}
