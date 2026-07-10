using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Movements;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Transfers;
using Jcd.Erp.Domain.Inventory.Warehouses;
using MediatR;

namespace Jcd.Erp.Application.Transfers.Commands.CreateInventoryTransfer;

public class CreateInventoryTransferHandler : IRequestHandler<CreateInventoryTransferCommand, Result<Guid>>
{
    private readonly IInventoryTransferRepository _transferRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IStockLevelRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryTransferHandler(
        IInventoryTransferRepository transferRepository,
        IInventoryMovementRepository movementRepository,
        IStockLevelRepository stockRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _transferRepository = transferRepository;
        _movementRepository = movementRepository;
        _stockRepository = stockRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateInventoryTransferCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        if (request.SourceWarehouseId == request.DestinationWarehouseId)
            return Result.Failure<Guid>("Transfer.SameWarehouse");

        var sourceWarehouse = await _warehouseRepository.GetByIdAsync(request.SourceWarehouseId, cancellationToken);
        if (sourceWarehouse is null)
            return Result.Failure<Guid>("Transfer.SourceWarehouseNotFound");

        if (!sourceWarehouse.IsActive)
            return Result.Failure<Guid>("Transfer.SourceWarehouseInactive");

        var destinationWarehouse = await _warehouseRepository.GetByIdAsync(request.DestinationWarehouseId, cancellationToken);
        if (destinationWarehouse is null)
            return Result.Failure<Guid>("Transfer.DestinationWarehouseNotFound");

        if (!destinationWarehouse.IsActive)
            return Result.Failure<Guid>("Transfer.DestinationWarehouseInactive");

        if (request.Lines.Count == 0)
            return Result.Failure<Guid>("Transfer.LinesRequired");

        var productIds = request.Lines.Select(l => l.ProductId).Distinct().ToList();
        if (productIds.Count != request.Lines.Count)
            return Result.Failure<Guid>("Transfer.DuplicateProduct");

        var lineData = new List<(Guid ProductId, decimal Quantity)>();
        foreach (var line in request.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product is null)
                return Result.Failure<Guid>("Transfer.ProductNotFound");

            if (!product.IsActive)
                return Result.Failure<Guid>("Transfer.ProductInactive");

            var sourceStock = await _stockRepository.GetByProductAndWarehouseAsync(
                line.ProductId,
                request.SourceWarehouseId,
                cancellationToken);

            if (sourceStock is null || sourceStock.QuantityOnHand <= 0)
                return Result.Failure<Guid>("Transfer.ProductNotInWarehouse");

            if (sourceStock.QuantityOnHand < line.Quantity)
                return Result.Failure<Guid>("Transfer.InsufficientStock");

            lineData.Add((line.ProductId, line.Quantity));
        }

        var transferDate = request.TransferDate ?? DateTime.UtcNow;
        var documentNumber = await _transferRepository.GetNextDocumentNumberAsync(cancellationToken);

        var transferResult = InventoryTransfer.Create(
            _tenant.TenantId,
            documentNumber,
            request.SourceWarehouseId,
            request.DestinationWarehouseId,
            transferDate,
            lineData,
            request.Notes);

        if (transferResult.IsFailure)
            return Result.Failure<Guid>(transferResult.Error);

        var transfer = transferResult.Value;
        var movementReference = transfer.DocumentNumber;
        var movementNotes = $"Transfer {transfer.DocumentNumber}";

        foreach (var line in transfer.Lines)
        {
            var outResult = await ApplyMovementAsync(
                line.ProductId,
                request.SourceWarehouseId,
                InventoryMovementTypes.Out,
                line.Quantity,
                transferDate,
                movementReference,
                movementNotes,
                cancellationToken);

            if (outResult.IsFailure)
                return Result.Failure<Guid>(outResult.Error);

            var inResult = await ApplyMovementAsync(
                line.ProductId,
                request.DestinationWarehouseId,
                InventoryMovementTypes.In,
                line.Quantity,
                transferDate,
                movementReference,
                movementNotes,
                cancellationToken);

            if (inResult.IsFailure)
                return Result.Failure<Guid>(inResult.Error);
        }

        await _transferRepository.AddAsync(transfer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(transfer.Id);
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
                return Result.Failure("Transfer.ProductNotInWarehouse");

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
            return applyResult;

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
