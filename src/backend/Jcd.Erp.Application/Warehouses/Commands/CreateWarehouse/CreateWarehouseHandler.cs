using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Commands.CreateWarehouse;

public class CreateWarehouseHandler : IRequestHandler<CreateWarehouseCommand, Result<Guid>>
{
    private readonly IWarehouseRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWarehouseHandler(
        IWarehouseRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        if (await _repository.GetByCodeAsync(request.Code, cancellationToken) is not null)
            return Result.Failure<Guid>("Warehouse.CodeAlreadyExists");

        var warehouseResult = Warehouse.Create(
            tenantId,
            request.Code,
            request.Name,
            request.Description,
            request.AddressLine1,
            request.City,
            request.StateOrProvince,
            request.CountryCode,
            request.IsDefault);

        if (warehouseResult.IsFailure)
            return Result.Failure<Guid>(warehouseResult.Error);

        if (request.IsDefault)
            await _repository.ClearDefaultExceptAsync(null, cancellationToken);

        var warehouse = warehouseResult.Value;
        await _repository.AddAsync(warehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(warehouse.Id);
    }
}
