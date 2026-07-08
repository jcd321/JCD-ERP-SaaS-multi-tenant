using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Commands.CreateLocation;

public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, Result<Guid>>
{
    private readonly IStorageLocationRepository _locationRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLocationHandler(
        IStorageLocationRepository locationRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse is null)
            return Result.Failure<Guid>("Location.WarehouseNotFound");

        if (request.ParentId.HasValue)
        {
            var parent = await _locationRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result.Failure<Guid>("Location.ParentNotFound");

            if (parent.WarehouseId != request.WarehouseId)
                return Result.Failure<Guid>("Location.ParentWarehouseMismatch");
        }

        if (await _locationRepository.GetByCodeAsync(request.WarehouseId, request.Code, cancellationToken) is not null)
            return Result.Failure<Guid>("Location.CodeAlreadyExists");

        var locationResult = StorageLocation.Create(
            tenantId,
            request.WarehouseId,
            request.Code,
            request.Name,
            request.Description,
            request.ParentId,
            request.LocationType);

        if (locationResult.IsFailure)
            return Result.Failure<Guid>(locationResult.Error);

        var location = locationResult.Value;
        await _locationRepository.AddAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(location.Id);
    }
}
