using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Identity;

public class UserSession : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public User User { get; private set; } = null!;

    private UserSession() { }

    public static UserSession Create(
        Guid tenantId,
        Guid userId,
        DateTime expiresAt,
        string? deviceInfo,
        string? ipAddress)
    {
        return new UserSession
        {
            TenantId = tenantId,
            UserId = userId,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsRevoked = false
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}
