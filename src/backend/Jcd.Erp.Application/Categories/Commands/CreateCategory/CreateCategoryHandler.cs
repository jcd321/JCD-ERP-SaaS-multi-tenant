using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Commands.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(
        ICategoryRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        if (request.ParentId.HasValue)
        {
            var parent = await _repository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result.Failure<Guid>("Category.ParentNotFound");
        }

        if (await _repository.GetByNameAsync(request.Name, request.ParentId, cancellationToken) is not null)
            return Result.Failure<Guid>("Category.NameAlreadyExists");

        var categoryResult = ProductCategory.Create(
            tenantId,
            request.Name,
            request.Description,
            request.ParentId);

        if (categoryResult.IsFailure)
            return Result.Failure<Guid>(categoryResult.Error);

        var category = categoryResult.Value;
        await _repository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(category.Id);
    }
}
