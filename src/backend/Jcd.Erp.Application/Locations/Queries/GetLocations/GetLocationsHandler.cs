using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Queries.GetLocations;

public class GetLocationsHandler : IRequestHandler<GetLocationsQuery, Result<PaginatedLocationsResult>>
{
    private readonly IStorageLocationRepository _repository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;

    public GetLocationsHandler(
        IStorageLocationRepository repository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedLocationsResult>> Handle(
        GetLocationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedLocationsResult>("Auth.TenantRequired");

        if (await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken) is null)
            return Result.Failure<PaginatedLocationsResult>("Location.WarehouseNotFound");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            request.WarehouseId,
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(l => new LocationDto(
                l.Id,
                l.WarehouseId,
                l.Code,
                l.Name,
                l.Description,
                l.ParentId,
                l.Parent?.Name,
                l.LocationType,
                l.IsActive))
            .ToList();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedLocationsResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
