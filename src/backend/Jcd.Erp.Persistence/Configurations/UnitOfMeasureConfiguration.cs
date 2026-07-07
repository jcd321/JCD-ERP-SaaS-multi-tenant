using Jcd.Erp.Domain.Catalog.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("units_of_measure");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(u => u.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.Property(u => u.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Symbol).HasColumnName("symbol").HasMaxLength(10);
        builder.Property(u => u.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(u => u.CreatedBy).HasColumnName("created_by");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        builder.Property(u => u.UpdatedBy).HasColumnName("updated_by");
        builder.Property(u => u.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at");
        builder.Property(u => u.DeletedBy).HasColumnName("deleted_by");

        builder.HasIndex(u => new { u.TenantId, u.Code })
            .IsUnique()
            .HasDatabaseName("ix_units_of_measure_tenant_id_code");

        builder.HasIndex(u => new { u.TenantId, u.IsDeleted })
            .HasDatabaseName("ix_units_of_measure_tenant_id_is_deleted");
    }
}
