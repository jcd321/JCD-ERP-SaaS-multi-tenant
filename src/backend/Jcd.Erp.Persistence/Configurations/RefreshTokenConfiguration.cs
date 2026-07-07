using Jcd.Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id).HasColumnName("id");
        builder.Property(rt => rt.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(rt => rt.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(rt => rt.TokenHash).HasColumnName("token_hash").HasMaxLength(512).IsRequired();
        builder.Property(rt => rt.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(rt => rt.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rt => rt.IsRevoked).HasColumnName("is_revoked").IsRequired();
        builder.Property(rt => rt.RevokedAt).HasColumnName("revoked_at");
        builder.Property(rt => rt.ReplacedByTokenHash).HasColumnName("replaced_by_token_hash").HasMaxLength(512);

        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique()
            .HasDatabaseName("ix_refresh_tokens_token_hash");

        builder.HasIndex(rt => new { rt.TenantId, rt.UserId })
            .HasDatabaseName("ix_refresh_tokens_tenant_id_user_id");

        builder.HasIndex(rt => new { rt.TenantId, rt.ExpiresAt })
            .HasDatabaseName("ix_refresh_tokens_tenant_id_expires_at");

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
