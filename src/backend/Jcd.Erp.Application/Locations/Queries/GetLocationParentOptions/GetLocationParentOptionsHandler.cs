using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Queries.GetLocationParentOptions;

public class GetLocationParentOptionsHandler
    : IRequestHandler<GetLocationParentOptionsQuery, Result<IReadOnlyList<LocationParentOptionDto>>>
{
    private readonly IStorageLocationRepository _repository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;

    public GetLocationParentOptionsHandler(
        IStorageLocationRepository repository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
    }

    public async Task<Result<IReadOnlyList<LocationParentOptionDto>>> Handle(
        GetLocationParentOptionsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<IReadOnlyList<LocationParentOptionDto>>("Auth.TenantRequired");

        if (await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken) is null)
            return Result.Failure<IReadOnlyList<LocationParentOptionDto>>("Location.WarehouseNotFound");

        var items = await _repository.GetParentOptionsAsync(
            request.WarehouseId,
            request.ExcludeId,
            cancellationToken);

        var dtos = items
            .Select(l => new LocationParentOptionDto(l.Id, l.Name, l.Code))
            .ToList();

        return Result.Success<IReadOnlyList<LocationParentOptionDto>>(dtos);
    }
}
