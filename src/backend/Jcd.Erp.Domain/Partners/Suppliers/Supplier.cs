using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Partners.Suppliers;

public class Supplier : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string LegalName { get; private set; } = string.Empty;
    public string? TradeName { get; private set; }
    public string? TaxId { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? MobilePhone { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? City { get; private set; }
    public string? StateOrProvince { get; private set; }
    public string? CountryCode { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    private Supplier() { }

    public static Result<Supplier> Create(
        Guid tenantId,
        string code,
        string legalName,
        string? tradeName = null,
        string? taxId = null,
        string? email = null,
        string? phone = null,
        string? mobilePhone = null,
        string? addressLine1 = null,
        string? city = null,
        string? stateOrProvince = null,
        string? countryCode = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<Supplier>("Supplier.TenantRequired");

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Supplier>("Supplier.CodeRequired");

        if (string.IsNullOrWhiteSpace(legalName))
            return Result.Failure<Supplier>("Supplier.LegalNameRequired");

        return Result.Success(new Supplier
        {
            TenantId = tenantId,
            Code = NormalizeCode(code),
            LegalName = legalName.Trim(),
            TradeName = NormalizeOptional(tradeName),
            TaxId = NormalizeTaxId(taxId),
            Email = NormalizeEmail(email),
            Phone = NormalizeOptional(phone),
            MobilePhone = NormalizeOptional(mobilePhone),
            AddressLine1 = NormalizeOptional(addressLine1),
            City = NormalizeOptional(city),
            StateOrProvince = NormalizeOptional(stateOrProvince),
            CountryCode = NormalizeCountryCode(countryCode),
            Notes = NormalizeOptional(notes),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result Update(
        string code,
        string legalName,
        string? tradeName,
        string? taxId,
        string? email,
        string? phone,
        string? mobilePhone,
        string? addressLine1,
        string? city,
        string? stateOrProvince,
        string? countryCode,
        string? notes,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure("Supplier.CodeRequired");

        if (string.IsNullOrWhiteSpace(legalName))
            return Result.Failure("Supplier.LegalNameRequired");

        Code = NormalizeCode(code);
        LegalName = legalName.Trim();
        TradeName = NormalizeOptional(tradeName);
        TaxId = NormalizeTaxId(taxId);
        Email = NormalizeEmail(email);
        Phone = NormalizeOptional(phone);
        MobilePhone = NormalizeOptional(mobilePhone);
        AddressLine1 = NormalizeOptional(addressLine1);
        City = NormalizeOptional(city);
        StateOrProvince = NormalizeOptional(stateOrProvince);
        CountryCode = NormalizeCountryCode(countryCode);
        Notes = NormalizeOptional(notes);
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeEmail(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();

    private static string? NormalizeTaxId(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    private static string? NormalizeCountryCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
