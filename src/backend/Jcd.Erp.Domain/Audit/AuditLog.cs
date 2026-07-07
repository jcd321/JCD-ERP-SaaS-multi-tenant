using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;

namespace Jcd.Erp.Domain.Audit;

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    Export = 6
}

public class AuditLog : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public AuditAction Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}
