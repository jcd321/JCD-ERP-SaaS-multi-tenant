using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Queries.GetBrands;

public record GetBrandsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<Result<PaginatedBrandsResult>>;

public record BrandDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive);

public record PaginatedBrandsResult(
    IReadOnlyList<BrandDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
