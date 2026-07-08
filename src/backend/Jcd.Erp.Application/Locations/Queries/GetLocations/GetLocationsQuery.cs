using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Queries.GetLocations;

public record GetLocationsQuery(
    Guid WarehouseId,
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<Result<PaginatedLocationsResult>>;

public record LocationDto(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    Guid? ParentId,
    string? ParentName,
    string? LocationType,
    bool IsActive);

public record PaginatedLocationsResult(
    IReadOnlyList<LocationDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
