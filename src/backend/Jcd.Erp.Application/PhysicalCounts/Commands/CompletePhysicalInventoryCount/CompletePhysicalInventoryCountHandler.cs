using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Movements;
using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using Jcd.Erp.Domain.Inventory.Stock;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.CompletePhysicalInventoryCount;

public class CompletePhysicalInventoryCountHandler : IRequestHandler<CompletePhysicalInventoryCountCommand, Result>
{
    private readonly IPhysicalInventoryCountRepository _countRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IStockLevelRepository _stockRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CompletePhysicalInventoryCountHandler(
        IPhysicalInventoryCountRepository countRepository,
        IInventoryMovementRepository movementRepository,
        IStockLevelRepository stockRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _countRepository = countRepository;
        _movementRepository = movementRepository;
        _stockRepository = stockRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompletePhysicalInventoryCountCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var count = await _countRepository.GetByIdAsync(request.CountId, cancellationToken);
        if (count is null)
            return Result.Failure("PhysicalCount.NotFound");

        var varianceLines = count.Lines.Where(l => l.HasVariance).ToList();
        var movementReference = count.DocumentNumber;
        var movementNotes = $"Physical count {count.DocumentNumber}";

        foreach (var line in varianceLines)
        {
            var delta = line.VarianceDelta!.Value;
            var movementType = delta > 0 ? InventoryMovementTypes.In : InventoryMovementTypes.Out;
            var quantity = Math.Abs(delta);

            var applyResult = await ApplyMovementAsync(
                line.ProductId,
                count.WarehouseId,
                movementType,
                quantity,
                count.CountDate,
                movementReference,
                movementNotes,
                cancellationToken);

            if (applyResult.IsFailure)
                return applyResult;
        }

        var completeResult = count.Complete();
        if (completeResult.IsFailure)
            return completeResult;

        _countRepository.Update(count);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
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
                return Result.Failure("PhysicalCount.InsufficientStock");

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
