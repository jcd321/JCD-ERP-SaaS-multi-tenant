using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Queries.GetUnits;

public record GetUnitsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<Result<PaginatedUnitsResult>>;

public record UnitOfMeasureDto(
    Guid Id,
    string Code,
    string Name,
    string? Symbol,
    bool IsActive);

public record PaginatedUnitsResult(
    IReadOnlyList<UnitOfMeasureDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
