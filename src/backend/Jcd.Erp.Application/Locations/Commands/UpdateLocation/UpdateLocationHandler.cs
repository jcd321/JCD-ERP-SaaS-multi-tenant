using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Commands.UpdateLocation;

public class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, Result>
{
    private readonly IStorageLocationRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLocationHandler(
        IStorageLocationRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var location = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (location is null)
            return Result.Failure("Location.NotFound");

        var existing = await _repository.GetByCodeAsync(location.WarehouseId, request.Code, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
            return Result.Failure("Location.CodeAlreadyExists");

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == request.Id)
                return Result.Failure("Location.CannotBeOwnParent");

            var parent = await _repository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result.Failure("Location.ParentNotFound");

            if (parent.WarehouseId != location.WarehouseId)
                return Result.Failure("Location.ParentWarehouseMismatch");
        }

        var updateResult = location.Update(
            request.Code,
            request.Name,
            request.Description,
            request.ParentId,
            request.LocationType,
            request.IsActive);

        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
