using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Queries.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<PaginatedCategoriesResult>>
{
    private readonly ICategoryRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetCategoriesHandler(ICategoryRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedCategoriesResult>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedCategoriesResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.ParentId,
                c.Parent?.Name,
                c.IsActive))
            .ToList();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedCategoriesResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
