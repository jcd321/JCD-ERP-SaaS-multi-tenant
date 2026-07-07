using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Commands.DeleteBrand;

public class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, Result>
{
    private readonly IBrandRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBrandHandler(
        IBrandRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var brand = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
            return Result.Failure("Brand.NotFound");

        _repository.Delete(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
