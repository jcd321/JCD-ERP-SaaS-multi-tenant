using Jcd.Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Module).HasColumnName("module").HasMaxLength(50).IsRequired();
        builder.Property(p => p.Action).HasColumnName("action").HasMaxLength(50).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(500);

        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("ix_permissions_code");

        builder.HasIndex(p => p.Module)
            .HasDatabaseName("ix_permissions_module");

        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
