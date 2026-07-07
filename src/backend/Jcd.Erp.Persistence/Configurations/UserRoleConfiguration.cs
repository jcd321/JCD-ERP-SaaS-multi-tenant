using Jcd.Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.UserId).HasColumnName("user_id");
        builder.Property(ur => ur.RoleId).HasColumnName("role_id");
        builder.Property(ur => ur.TenantId).HasColumnName("tenant_id").IsRequired();

        builder.HasIndex(ur => new { ur.TenantId, ur.UserId })
            .HasDatabaseName("ix_user_roles_tenant_id_user_id");

        builder.HasIndex(ur => new { ur.TenantId, ur.RoleId })
            .HasDatabaseName("ix_user_roles_tenant_id_role_id");
    }
}
