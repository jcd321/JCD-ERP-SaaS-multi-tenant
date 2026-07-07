using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly ICategoryRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandler(
        ICategoryRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var category = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure("Category.NotFound");

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == category.Id)
                return Result.Failure("Category.CannotBeOwnParent");

            var parent = await _repository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result.Failure("Category.ParentNotFound");
        }

        var existingByName = await _repository.GetByNameAsync(request.Name, request.ParentId, cancellationToken);
        if (existingByName is not null && existingByName.Id != category.Id)
            return Result.Failure("Category.NameAlreadyExists");

        var updateResult = category.Update(
            request.Name,
            request.Description,
            request.ParentId,
            request.IsActive);

        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
