using Jcd.Erp.Application.Common.Interfaces;

namespace Jcd.Erp.Persistence.Context;

/// <summary>
/// Bridges request-scoped tenant resolution into <see cref="ITenantContext"/>.
/// Middleware or filters call <see cref="ApplyFrom"/> after resolving the tenant.
/// </summary>
public sealed class TenantProvider
{
    private readonly ITenantContext _tenantContext;

    public TenantProvider(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public Guid? TenantId => _tenantContext.TenantId;

    public void SetTenant(Guid? tenantId) => _tenantContext.TenantId = tenantId;

    public void ApplyFrom(ICurrentTenantService currentTenantService)
    {
        _tenantContext.TenantId = currentTenantService.HasTenant
            ? currentTenantService.TenantId
            : null;
    }
}
