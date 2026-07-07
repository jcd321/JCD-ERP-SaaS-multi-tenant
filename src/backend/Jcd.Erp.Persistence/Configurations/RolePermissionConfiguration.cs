using Jcd.Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Property(rp => rp.RoleId).HasColumnName("role_id");
        builder.Property(rp => rp.PermissionId).HasColumnName("permission_id");

        builder.HasIndex(rp => rp.PermissionId)
            .HasDatabaseName("ix_role_permissions_permission_id");
    }
}
