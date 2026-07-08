using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Queries.GetWarehouses;

public record GetWarehousesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<Result<PaginatedWarehousesResult>>;

public record WarehouseDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string? AddressLine1,
    string? City,
    string? StateOrProvince,
    string? CountryCode,
    bool IsDefault,
    bool IsActive);

public record PaginatedWarehousesResult(
    IReadOnlyList<WarehouseDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
