using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Movements;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Movements.Queries.GetInventoryMovements;

public class GetInventoryMovementsHandler : IRequestHandler<GetInventoryMovementsQuery, Result<PaginatedInventoryMovementsResult>>
{
    private readonly IInventoryMovementRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetInventoryMovementsHandler(IInventoryMovementRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedInventoryMovementsResult>> Handle(
        GetInventoryMovementsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedInventoryMovementsResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.WarehouseId,
            request.ProductId,
            request.MovementType,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = items
            .Select(m => new InventoryMovementDto(
                m.Id,
                m.DocumentNumber,
                m.ProductId,
                m.Product.Sku,
                m.Product.Name,
                m.Product.Unit.Symbol,
                m.WarehouseId,
                m.Warehouse.Code,
                m.Warehouse.Name,
                m.MovementType,
                m.Quantity,
                m.QuantityBefore,
                m.QuantityAfter,
                m.Reference,
                m.Notes,
                m.MovementDate,
                m.CreatedAt))
            .ToList();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedInventoryMovementsResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
