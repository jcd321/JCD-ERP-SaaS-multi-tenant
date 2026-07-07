using Jcd.Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(t => t.TokenHash).HasColumnName("token_hash").HasMaxLength(512).IsRequired();
        builder.Property(t => t.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(t => t.IsUsed).HasColumnName("is_used").IsRequired();
        builder.Property(t => t.UsedAt).HasColumnName("used_at");

        builder.HasIndex(t => t.TokenHash)
            .IsUnique()
            .HasDatabaseName("ix_password_reset_tokens_token_hash");

        builder.HasIndex(t => new { t.TenantId, t.UserId })
            .HasDatabaseName("ix_password_reset_tokens_tenant_id_user_id");

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
