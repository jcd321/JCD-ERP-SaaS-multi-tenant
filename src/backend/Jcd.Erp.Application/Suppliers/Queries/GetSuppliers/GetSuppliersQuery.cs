using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<Result<PaginatedSuppliersResult>>;

public record SupplierDto(
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

public record PaginatedSuppliersResult(
    IReadOnlyList<SupplierDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
