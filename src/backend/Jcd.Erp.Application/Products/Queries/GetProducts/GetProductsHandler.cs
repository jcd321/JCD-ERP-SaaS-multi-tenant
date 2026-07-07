using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<PaginatedProductsResult>>
{
    private readonly IProductRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetProductsHandler(IProductRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedProductsResult>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedProductsResult>("Auth.TenantRequired");

        var page = Math.Clamp(request.Page, 1, int.MaxValue);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(p => new ProductDto(
                p.Id,
                p.Sku,
                p.Name,
                p.Description,
                p.CategoryId,
                p.Category.Name,
                p.BrandId,
                p.Brand?.Name,
                p.UnitId,
                p.Unit.Name,
                p.IsActive))
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result.Success(new PaginatedProductsResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
