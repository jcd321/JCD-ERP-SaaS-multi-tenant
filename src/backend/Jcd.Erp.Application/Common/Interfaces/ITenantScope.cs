namespace Jcd.Erp.Application.Common.Interfaces;

/// <summary>
/// Sets the active tenant for the current request scope (DbContext query filters).
/// Used by auth handlers before tenant-scoped queries when JWT is not yet available.
/// </summary>
public interface ITenantScope
{
    void SetTenant(Guid tenantId);

    void Clear();
}
