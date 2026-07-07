using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Customers.Queries.GetCustomers;

public record GetCustomersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<Result<PaginatedCustomersResult>>;

public record CustomerDto(
    Guid Id,
    string Code,
    string LegalName,
    string? TradeName,
    string? TaxId,
    string? Email,
    string? Phone,
    string? MobilePhone,
    string? AddressLine1,
    string? City,
    string? StateOrProvince,
    string? CountryCode,
    string? Notes,
    bool IsActive);

public record PaginatedCustomersResult(
    IReadOnlyList<CustomerDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
