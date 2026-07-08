using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Commands.DeleteWarehouse;

public class DeleteWarehouseHandler : IRequestHandler<DeleteWarehouseCommand, Result>
{
    private readonly IWarehouseRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWarehouseHandler(
        IWarehouseRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var warehouse = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse is null)
            return Result.Failure("Warehouse.NotFound");

        if (await _repository.HasLocationsAsync(warehouse.Id, cancellationToken))
            return Result.Failure("Warehouse.HasLocations");

        _repository.Delete(warehouse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
