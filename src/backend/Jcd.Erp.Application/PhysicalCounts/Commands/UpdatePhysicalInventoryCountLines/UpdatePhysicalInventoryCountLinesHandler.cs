using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.UpdatePhysicalInventoryCountLines;

public class UpdatePhysicalInventoryCountLinesHandler : IRequestHandler<UpdatePhysicalInventoryCountLinesCommand, Result>
{
    private readonly IPhysicalInventoryCountRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePhysicalInventoryCountLinesHandler(
        IPhysicalInventoryCountRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdatePhysicalInventoryCountLinesCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var count = await _repository.GetByIdAsync(request.CountId, cancellationToken);
        if (count is null)
            return Result.Failure("PhysicalCount.NotFound");

        var updates = request.Lines
            .Select(l => (l.LineId, l.CountedQuantity))
            .ToList();

        var updateResult = count.UpdateLineCounts(updates);
        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(count);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
