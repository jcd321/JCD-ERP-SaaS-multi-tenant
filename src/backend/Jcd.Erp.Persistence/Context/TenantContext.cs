namespace Jcd.Erp.Persistence.Context;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; set; }
}
