using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Commands.DeleteLocation;

public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, Result>
{
    private readonly IStorageLocationRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLocationHandler(
        IStorageLocationRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var location = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (location is null)
            return Result.Failure("Location.NotFound");

        if (await _repository.HasChildrenAsync(location.Id, cancellationToken))
            return Result.Failure("Location.HasChildren");

        _repository.Delete(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
