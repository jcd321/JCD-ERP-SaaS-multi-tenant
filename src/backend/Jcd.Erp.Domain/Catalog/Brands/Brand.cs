using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Catalog.Brands;

public class Brand : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Brand() { }

    public static Result<Brand> Create(
        Guid tenantId,
        string code,
        string name,
        string? description = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<Brand>("Brand.TenantRequired");

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Brand>("Brand.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Brand>("Brand.NameRequired");

        return Result.Success(new Brand
        {
            TenantId = tenantId,
            Code = NormalizeCode(code),
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result Update(string code, string name, string? description, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure("Brand.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Brand.NameRequired");

        Code = NormalizeCode(code);
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
}
