using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Commands.UpdateBrand;

public class UpdateBrandHandler : IRequestHandler<UpdateBrandCommand, Result>
{
    private readonly IBrandRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBrandHandler(
        IBrandRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var brand = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
            return Result.Failure("Brand.NotFound");

        var existing = await _repository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
            return Result.Failure("Brand.CodeAlreadyExists");

        var updateResult = brand.Update(request.Code, request.Name, request.Description, request.IsActive);
        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
