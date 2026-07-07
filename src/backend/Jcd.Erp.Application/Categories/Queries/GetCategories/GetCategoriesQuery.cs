using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<Result<PaginatedCategoriesResult>>;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentId,
    string? ParentName,
    bool IsActive);

public record PaginatedCategoriesResult(
    IReadOnlyList<CategoryDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
