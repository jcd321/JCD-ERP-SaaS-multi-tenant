using Jcd.Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(us => us.Id);

        builder.Property(us => us.Id).HasColumnName("id");
        builder.Property(us => us.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(us => us.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(us => us.DeviceInfo).HasColumnName("device_info").HasMaxLength(500);
        builder.Property(us => us.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
        builder.Property(us => us.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(us => us.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(us => us.IsRevoked).HasColumnName("is_revoked").IsRequired();
        builder.Property(us => us.RevokedAt).HasColumnName("revoked_at");

        builder.HasIndex(us => new { us.TenantId, us.UserId })
            .HasDatabaseName("ix_user_sessions_tenant_id_user_id");

        builder.HasIndex(us => new { us.TenantId, us.ExpiresAt })
            .HasDatabaseName("ix_user_sessions_tenant_id_expires_at");

        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
