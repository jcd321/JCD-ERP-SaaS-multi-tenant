using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Warehouses;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalCountLookups;

public record GetPhysicalCountLookupsQuery : IRequest<Result<PhysicalCountLookupsDto>>;

public record PhysicalCountWarehouseOption(Guid Id, string Code, string Name);

public record PhysicalCountLookupsDto(IReadOnlyList<PhysicalCountWarehouseOption> Warehouses);

public class GetPhysicalCountLookupsHandler : IRequestHandler<GetPhysicalCountLookupsQuery, Result<PhysicalCountLookupsDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;

    public GetPhysicalCountLookupsHandler(IWarehouseRepository warehouseRepository, ICurrentTenantService tenant)
    {
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
    }

    public async Task<Result<PhysicalCountLookupsDto>> Handle(
        GetPhysicalCountLookupsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PhysicalCountLookupsDto>("Auth.TenantRequired");

        var (warehouses, _) = await _warehouseRepository.GetPagedAsync(1, 200, null, true, cancellationToken);

        var options = warehouses
            .OrderBy(w => w.Code)
            .Select(w => new PhysicalCountWarehouseOption(w.Id, w.Code, w.Name))
            .ToList();

        return Result.Success(new PhysicalCountLookupsDto(options));
    }
}
