using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Commands.CreateBrand;

public class CreateBrandHandler : IRequestHandler<CreateBrandCommand, Result<Guid>>
{
    private readonly IBrandRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBrandHandler(
        IBrandRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        if (await _repository.GetByCodeAsync(request.Code, cancellationToken) is not null)
            return Result.Failure<Guid>("Brand.CodeAlreadyExists");

        var brandResult = Brand.Create(tenantId, request.Code, request.Name, request.Description);
        if (brandResult.IsFailure)
            return Result.Failure<Guid>(brandResult.Error);

        var brand = brandResult.Value;
        await _repository.AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(brand.Id);
    }
}
