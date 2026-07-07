using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Queries.GetCategoryParentOptions;

public class GetCategoryParentOptionsHandler
    : IRequestHandler<GetCategoryParentOptionsQuery, Result<IReadOnlyList<CategoryParentOptionDto>>>
{
    private readonly ICategoryRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetCategoryParentOptionsHandler(ICategoryRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<IReadOnlyList<CategoryParentOptionDto>>> Handle(
        GetCategoryParentOptionsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<IReadOnlyList<CategoryParentOptionDto>>("Auth.TenantRequired");

        var categories = await _repository.GetAllForParentSelectAsync(request.ExcludeId, cancellationToken);
        var dtos = categories.Select(c => new CategoryParentOptionDto(c.Id, c.Name)).ToList();

        return Result.Success<IReadOnlyList<CategoryParentOptionDto>>(dtos);
    }
}
