namespace Jcd.Erp.Persistence.Context;

public interface ITenantContext
{
    Guid? TenantId { get; set; }
}
