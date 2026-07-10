using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCounts;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCountById;

public class GetPhysicalInventoryCountByIdHandler : IRequestHandler<GetPhysicalInventoryCountByIdQuery, Result<PhysicalInventoryCountDto>>
{
    private readonly IPhysicalInventoryCountRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetPhysicalInventoryCountByIdHandler(IPhysicalInventoryCountRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PhysicalInventoryCountDto>> Handle(
        GetPhysicalInventoryCountByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PhysicalInventoryCountDto>("Auth.TenantRequired");

        var count = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (count is null)
            return Result.Failure<PhysicalInventoryCountDto>("PhysicalCount.NotFound");

        return Result.Success(PhysicalInventoryCountMapper.MapToDto(count));
    }
}
