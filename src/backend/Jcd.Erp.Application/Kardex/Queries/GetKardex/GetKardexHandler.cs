using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Movements;
using MediatR;

namespace Jcd.Erp.Application.Kardex.Queries.GetKardex;

public class GetKardexHandler : IRequestHandler<GetKardexQuery, Result<PaginatedKardexResult>>
{
    private readonly IInventoryMovementRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetKardexHandler(IInventoryMovementRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedKardexResult>> Handle(
        GetKardexQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedKardexResult>("Auth.TenantRequired");

        if (request.ProductId == Guid.Empty)
            return Result.Failure<PaginatedKardexResult>("Kardex.ProductRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 50 : request.PageSize;

        var (items, totalCount) = await _repository.GetKardexPagedAsync(
            request.ProductId,
            page,
            pageSize,
            request.WarehouseId,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = items
            .Select(m => new KardexEntryDto(
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

        return Result.Success(new PaginatedKardexResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
