using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Configuration;

public class TenantSetting : BaseAuditableEntity
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private TenantSetting() { }

    public static TenantSetting Create(Guid tenantId, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required", nameof(key));

        return new TenantSetting
        {
            TenantId = tenantId,
            Key = key.Trim().ToLowerInvariant(),
            Value = value,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateValue(string value)
    {
        Value = value;
        UpdatedAt = DateTime.UtcNow;
    }
}
