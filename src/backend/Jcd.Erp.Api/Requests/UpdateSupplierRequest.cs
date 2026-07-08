namespace Jcd.Erp.Api.Requests;

public sealed class UpdateSupplierRequest
{
    public string Code { get; init; } = string.Empty;
    public string LegalName { get; init; } = string.Empty;
    public string? TradeName { get; init; }
    public string? TaxId { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? MobilePhone { get; init; }
    public string? AddressLine1 { get; init; }
    public string? City { get; init; }
    public string? StateOrProvince { get; init; }
    public string? CountryCode { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
}
