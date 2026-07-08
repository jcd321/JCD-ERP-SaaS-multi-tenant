using Jcd.Erp.Domain.Inventory.Movements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("inventory_movements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(m => m.DocumentNumber).HasColumnName("document_number").HasMaxLength(30).IsRequired();
        builder.Property(m => m.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(m => m.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(m => m.MovementType).HasColumnName("movement_type").HasMaxLength(10).IsRequired();
        builder.Property(m => m.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(m => m.QuantityBefore).HasColumnName("quantity_before").HasPrecision(18, 4).IsRequired();
        builder.Property(m => m.QuantityAfter).HasColumnName("quantity_after").HasPrecision(18, 4).IsRequired();
        builder.Property(m => m.Reference).HasColumnName("reference").HasMaxLength(100);
        builder.Property(m => m.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(m => m.MovementDate).HasColumnName("movement_date").IsRequired();
        builder.Property(m => m.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(m => m.CreatedBy).HasColumnName("created_by");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
        builder.Property(m => m.UpdatedBy).HasColumnName("updated_by");
        builder.Property(m => m.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(m => m.DeletedAt).HasColumnName("deleted_at");
        builder.Property(m => m.DeletedBy).HasColumnName("deleted_by");

        builder.HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Warehouse)
            .WithMany()
            .HasForeignKey(m => m.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.TenantId, m.DocumentNumber })
            .IsUnique()
            .HasDatabaseName("ix_inventory_movements_tenant_id_document_number");

        builder.HasIndex(m => new { m.TenantId, m.MovementDate })
            .HasDatabaseName("ix_inventory_movements_tenant_id_movement_date");

        builder.HasIndex(m => new { m.TenantId, m.WarehouseId, m.IsDeleted })
            .HasDatabaseName("ix_inventory_movements_tenant_id_warehouse_id_is_deleted");
    }
}
