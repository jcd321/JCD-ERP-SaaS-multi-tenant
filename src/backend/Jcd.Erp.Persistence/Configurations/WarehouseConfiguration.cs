using Jcd.Erp.Domain.Inventory.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("warehouses");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id).HasColumnName("id");
        builder.Property(w => w.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(w => w.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.Property(w => w.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(w => w.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(w => w.AddressLine1).HasColumnName("address_line1").HasMaxLength(200);
        builder.Property(w => w.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(w => w.StateOrProvince).HasColumnName("state_or_province").HasMaxLength(100);
        builder.Property(w => w.CountryCode).HasColumnName("country_code").HasMaxLength(2);
        builder.Property(w => w.IsDefault).HasColumnName("is_default").IsRequired();
        builder.Property(w => w.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(w => w.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(w => w.CreatedBy).HasColumnName("created_by");
        builder.Property(w => w.UpdatedAt).HasColumnName("updated_at");
        builder.Property(w => w.UpdatedBy).HasColumnName("updated_by");
        builder.Property(w => w.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(w => w.DeletedAt).HasColumnName("deleted_at");
        builder.Property(w => w.DeletedBy).HasColumnName("deleted_by");

        builder.HasMany(w => w.Locations)
            .WithOne(l => l.Warehouse)
            .HasForeignKey(l => l.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => new { w.TenantId, w.Code })
            .IsUnique()
            .HasDatabaseName("ix_warehouses_tenant_id_code");

        builder.HasIndex(w => new { w.TenantId, w.IsDeleted })
            .HasDatabaseName("ix_warehouses_tenant_id_is_deleted");
    }
}
