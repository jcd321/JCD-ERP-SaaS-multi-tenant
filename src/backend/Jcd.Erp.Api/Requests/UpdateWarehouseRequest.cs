namespace Jcd.Erp.Api.Requests;

public record UpdateWarehouseRequest(
    string Code,
    string Name,
    string? Description,
    string? AddressLine1,
    string? City,
    string? StateOrProvince,
    string? CountryCode,
    bool IsDefault,
    bool IsActive);
