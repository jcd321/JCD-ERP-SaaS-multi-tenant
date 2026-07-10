using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.CancelPhysicalInventoryCount;

public class CancelPhysicalInventoryCountHandler : IRequestHandler<CancelPhysicalInventoryCountCommand, Result>
{
    private readonly IPhysicalInventoryCountRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPhysicalInventoryCountHandler(
        IPhysicalInventoryCountRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelPhysicalInventoryCountCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var count = await _repository.GetByIdAsync(request.CountId, cancellationToken);
        if (count is null)
            return Result.Failure("PhysicalCount.NotFound");

        var cancelResult = count.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult;

        _repository.Update(count);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
