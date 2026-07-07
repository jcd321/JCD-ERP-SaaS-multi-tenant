using Jcd.Erp.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class TenantSettingConfiguration : IEntityTypeConfiguration<TenantSetting>
{
    public void Configure(EntityTypeBuilder<TenantSetting> builder)
    {
        builder.ToTable("tenant_settings");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Id).HasColumnName("id");
        builder.Property(ts => ts.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(ts => ts.Key).HasColumnName("key").HasMaxLength(100).IsRequired();
        builder.Property(ts => ts.Value).HasColumnName("value").HasMaxLength(2000).IsRequired();
        builder.Property(ts => ts.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(ts => ts.CreatedBy).HasColumnName("created_by");
        builder.Property(ts => ts.UpdatedAt).HasColumnName("updated_at");
        builder.Property(ts => ts.UpdatedBy).HasColumnName("updated_by");
        builder.Property(ts => ts.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(ts => ts.DeletedAt).HasColumnName("deleted_at");
        builder.Property(ts => ts.DeletedBy).HasColumnName("deleted_by");

        builder.HasIndex(ts => new { ts.TenantId, ts.Key })
            .IsUnique()
            .HasDatabaseName("ix_tenant_settings_tenant_id_key");

        builder.HasIndex(ts => new { ts.TenantId, ts.IsDeleted })
            .HasDatabaseName("ix_tenant_settings_tenant_id_is_deleted");
    }
}
