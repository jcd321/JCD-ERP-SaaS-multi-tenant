using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Catalog.Units;

public class UnitOfMeasure : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Symbol { get; private set; }
    public bool IsActive { get; private set; }

    private UnitOfMeasure() { }

    public static Result<UnitOfMeasure> Create(
        Guid tenantId,
        string code,
        string name,
        string? symbol = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<UnitOfMeasure>("Unit.TenantRequired");

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<UnitOfMeasure>("Unit.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<UnitOfMeasure>("Unit.NameRequired");

        return Result.Success(new UnitOfMeasure
        {
            TenantId = tenantId,
            Code = NormalizeCode(code),
            Name = name.Trim(),
            Symbol = string.IsNullOrWhiteSpace(symbol) ? null : symbol.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result Update(string code, string name, string? symbol, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure("Unit.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Unit.NameRequired");

        Code = NormalizeCode(code);
        Name = name.Trim();
        Symbol = string.IsNullOrWhiteSpace(symbol) ? null : symbol.Trim();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
}
