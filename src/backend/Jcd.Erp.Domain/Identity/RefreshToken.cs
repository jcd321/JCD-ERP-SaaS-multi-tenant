using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Identity;

public class RefreshToken : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public User User { get; private set; } = null!;

    private RefreshToken() { }

    public static RefreshToken Create(Guid tenantId, Guid userId, string tokenHash, DateTime expiresAt)
    {
        return new RefreshToken
        {
            TenantId = tenantId,
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? replacedByTokenHash = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
