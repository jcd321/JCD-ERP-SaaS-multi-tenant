using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Commands.UpdateUnit;

public class UpdateUnitHandler : IRequestHandler<UpdateUnitCommand, Result>
{
    private readonly IUnitOfMeasureRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUnitHandler(
        IUnitOfMeasureRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var unit = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure("Unit.NotFound");

        var existingByCode = await _repository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingByCode is not null && existingByCode.Id != unit.Id)
            return Result.Failure("Unit.CodeAlreadyExists");

        var updateResult = unit.Update(request.Code, request.Name, request.Symbol, request.IsActive);
        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
