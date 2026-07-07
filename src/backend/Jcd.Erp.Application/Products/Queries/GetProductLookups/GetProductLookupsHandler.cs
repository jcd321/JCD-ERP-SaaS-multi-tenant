using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Queries.GetProductLookups;

public class GetProductLookupsHandler : IRequestHandler<GetProductLookupsQuery, Result<ProductLookupsResult>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfMeasureRepository _unitRepository;
    private readonly ICurrentTenantService _tenant;

    public GetProductLookupsHandler(
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IUnitOfMeasureRepository unitRepository,
        ICurrentTenantService tenant)
    {
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _unitRepository = unitRepository;
        _tenant = tenant;
    }

    public async Task<Result<ProductLookupsResult>> Handle(
        GetProductLookupsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<ProductLookupsResult>("Auth.TenantRequired");

        var categories = await _categoryRepository.GetAllForParentSelectAsync(null, cancellationToken);
        var (brands, _) = await _brandRepository.GetPagedAsync(1, 500, null, true, cancellationToken);
        var (units, _) = await _unitRepository.GetPagedAsync(1, 500, null, true, cancellationToken);

        return Result.Success(new ProductLookupsResult(
            categories.Select(c => new LookupOptionDto(c.Id, c.Name)).ToList(),
            brands.Select(b => new LookupOptionDto(b.Id, b.Name)).ToList(),
            units.Select(u => new LookupOptionDto(u.Id, u.Name)).ToList()));
    }
}
