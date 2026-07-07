using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Commands.DeleteUnit;

public class DeleteUnitHandler : IRequestHandler<DeleteUnitCommand, Result>
{
    private readonly IUnitOfMeasureRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUnitHandler(
        IUnitOfMeasureRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var unit = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure("Unit.NotFound");

        _repository.Delete(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
