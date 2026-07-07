using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Commands.CreateUnit;

public class CreateUnitHandler : IRequestHandler<CreateUnitCommand, Result<Guid>>
{
    private readonly IUnitOfMeasureRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUnitHandler(
        IUnitOfMeasureRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        if (await _repository.GetByCodeAsync(request.Code, cancellationToken) is not null)
            return Result.Failure<Guid>("Unit.CodeAlreadyExists");

        var unitResult = UnitOfMeasure.Create(tenantId, request.Code, request.Name, request.Symbol);
        if (unitResult.IsFailure)
            return Result.Failure<Guid>(unitResult.Error);

        var unit = unitResult.Value;
        await _repository.AddAsync(unit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(unit.Id);
    }
}
