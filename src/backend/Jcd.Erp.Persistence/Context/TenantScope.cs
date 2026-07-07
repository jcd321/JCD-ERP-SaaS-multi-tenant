using Jcd.Erp.Application.Common.Interfaces;

namespace Jcd.Erp.Persistence.Context;

public sealed class TenantScope : ITenantScope
{
    private readonly ITenantContext _tenantContext;

    public TenantScope(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public void SetTenant(Guid tenantId) => _tenantContext.TenantId = tenantId;

    public void Clear() => _tenantContext.TenantId = null;
}
