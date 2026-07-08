using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Inventory.Warehouses;

public class Warehouse : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? City { get; private set; }
    public string? StateOrProvince { get; private set; }
    public string? CountryCode { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<StorageLocation> Locations { get; private set; } = [];

    private Warehouse() { }

    public static Result<Warehouse> Create(
        Guid tenantId,
        string code,
        string name,
        string? description = null,
        string? addressLine1 = null,
        string? city = null,
        string? stateOrProvince = null,
        string? countryCode = null,
        bool isDefault = false)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<Warehouse>("Warehouse.TenantRequired");

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Warehouse>("Warehouse.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Warehouse>("Warehouse.NameRequired");

        return Result.Success(new Warehouse
        {
            TenantId = tenantId,
            Code = NormalizeCode(code),
            Name = name.Trim(),
            Description = NormalizeOptional(description),
            AddressLine1 = NormalizeOptional(addressLine1),
            City = NormalizeOptional(city),
            StateOrProvince = NormalizeOptional(stateOrProvince),
            CountryCode = NormalizeCountryCode(countryCode),
            IsDefault = isDefault,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result Update(
        string code,
        string name,
        string? description,
        string? addressLine1,
        string? city,
        string? stateOrProvince,
        string? countryCode,
        bool isDefault,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure("Warehouse.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Warehouse.NameRequired");

        Code = NormalizeCode(code);
        Name = name.Trim();
        Description = NormalizeOptional(description);
        AddressLine1 = NormalizeOptional(addressLine1);
        City = NormalizeOptional(city);
        StateOrProvince = NormalizeOptional(stateOrProvince);
        CountryCode = NormalizeCountryCode(countryCode);
        IsDefault = isDefault;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void SetDefault(bool isDefault) => IsDefault = isDefault;

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeCountryCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
