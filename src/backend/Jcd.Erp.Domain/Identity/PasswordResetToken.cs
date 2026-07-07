using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Identity;

public class PasswordResetToken : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }

    public User User { get; private set; } = null!;

    private PasswordResetToken() { }

    public static PasswordResetToken Create(Guid tenantId, Guid userId, string tokenHash, DateTime expiresAt) =>
        new()
        {
            TenantId = tenantId,
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsUsed && !IsExpired;

    public void MarkUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }
}
