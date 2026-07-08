using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseHandler : IRequestHandler<UpdateWarehouseCommand, Result>
{
    private readonly IWarehouseRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWarehouseHandler(
        IWarehouseRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var warehouse = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse is null)
            return Result.Failure("Warehouse.NotFound");

        var existing = await _repository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
            return Result.Failure("Warehouse.CodeAlreadyExists");

        if (request.IsDefault)
            await _repository.ClearDefaultExceptAsync(request.Id, cancellationToken);

        var updateResult = warehouse.Update(
            request.Code,
            request.Name,
            request.Description,
            request.AddressLine1,
            request.City,
            request.StateOrProvince,
            request.CountryCode,
            request.IsDefault,
            request.IsActive);

        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(warehouse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
