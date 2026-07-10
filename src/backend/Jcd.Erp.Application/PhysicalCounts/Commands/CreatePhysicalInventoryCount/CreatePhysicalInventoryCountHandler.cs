using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Warehouses;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.CreatePhysicalInventoryCount;

public class CreatePhysicalInventoryCountHandler : IRequestHandler<CreatePhysicalInventoryCountCommand, Result<Guid>>
{
    private readonly IPhysicalInventoryCountRepository _countRepository;
    private readonly IStockLevelRepository _stockRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePhysicalInventoryCountHandler(
        IPhysicalInventoryCountRepository countRepository,
        IStockLevelRepository stockRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _countRepository = countRepository;
        _stockRepository = stockRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePhysicalInventoryCountCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse is null)
            return Result.Failure<Guid>("PhysicalCount.WarehouseNotFound");

        if (!warehouse.IsActive)
            return Result.Failure<Guid>("PhysicalCount.WarehouseInactive");

        var (stockItems, _) = await _stockRepository.GetPagedAsync(
            1,
            500,
            null,
            request.WarehouseId,
            null,
            null,
            cancellationToken);

        if (stockItems.Count == 0)
            return Result.Failure<Guid>("PhysicalCount.NoStockInWarehouse");

        var lines = stockItems
            .OrderBy(s => s.ProductId)
            .Select(s => (s.ProductId, s.QuantityOnHand))
            .ToList();

        var countDate = request.CountDate ?? DateTime.UtcNow;
        var documentNumber = await _countRepository.GetNextDocumentNumberAsync(cancellationToken);

        var countResult = PhysicalInventoryCount.Create(
            _tenant.TenantId,
            documentNumber,
            request.WarehouseId,
            countDate,
            lines,
            request.Notes);

        if (countResult.IsFailure)
            return Result.Failure<Guid>(countResult.Error);

        await _countRepository.AddAsync(countResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(countResult.Value.Id);
    }
}
