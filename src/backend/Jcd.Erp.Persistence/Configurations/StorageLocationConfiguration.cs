using Jcd.Erp.Domain.Inventory.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.ToTable("storage_locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).HasColumnName("id");
        builder.Property(l => l.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(l => l.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(l => l.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.Property(l => l.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(l => l.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(l => l.ParentId).HasColumnName("parent_id");
        builder.Property(l => l.LocationType).HasColumnName("location_type").HasMaxLength(30);
        builder.Property(l => l.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(l => l.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(l => l.CreatedBy).HasColumnName("created_by");
        builder.Property(l => l.UpdatedAt).HasColumnName("updated_at");
        builder.Property(l => l.UpdatedBy).HasColumnName("updated_by");
        builder.Property(l => l.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(l => l.DeletedAt).HasColumnName("deleted_at");
        builder.Property(l => l.DeletedBy).HasColumnName("deleted_by");

        builder.HasOne(l => l.Parent)
            .WithMany(l => l.Children)
            .HasForeignKey(l => l.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.TenantId, l.WarehouseId, l.Code })
            .IsUnique()
            .HasDatabaseName("ix_storage_locations_tenant_id_warehouse_id_code");

        builder.HasIndex(l => new { l.TenantId, l.WarehouseId, l.IsDeleted })
            .HasDatabaseName("ix_storage_locations_tenant_id_warehouse_id_is_deleted");
    }
}
