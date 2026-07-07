using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Queries.GetBrands;

public class GetBrandsHandler : IRequestHandler<GetBrandsQuery, Result<PaginatedBrandsResult>>
{
    private readonly IBrandRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetBrandsHandler(IBrandRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedBrandsResult>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedBrandsResult>("Auth.TenantRequired");

        var page = Math.Clamp(request.Page, 1, int.MaxValue);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(b => new BrandDto(b.Id, b.Code, b.Name, b.Description, b.IsActive))
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result.Success(new PaginatedBrandsResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
