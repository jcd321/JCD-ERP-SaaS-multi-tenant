namespace Jcd.Erp.Api.Requests;

public record UpdateCustomerRequest(
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
