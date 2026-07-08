using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Queries.GetWarehouses;

public class GetWarehousesHandler : IRequestHandler<GetWarehousesQuery, Result<PaginatedWarehousesResult>>
{
    private readonly IWarehouseRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetWarehousesHandler(IWarehouseRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedWarehousesResult>> Handle(
        GetWarehousesQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedWarehousesResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(w => new WarehouseDto(
                w.Id,
                w.Code,
                w.Name,
                w.Description,
                w.AddressLine1,
                w.City,
                w.StateOrProvince,
                w.CountryCode,
                w.IsDefault,
                w.IsActive))
            .ToList();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedWarehousesResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
