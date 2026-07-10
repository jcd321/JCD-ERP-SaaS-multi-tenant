using Jcd.Erp.Domain.Inventory.Adjustments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class InventoryAdjustmentConfiguration : IEntityTypeConfiguration<InventoryAdjustment>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustment> builder)
    {
        builder.ToTable("inventory_adjustments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(a => a.DocumentNumber).HasColumnName("document_number").HasMaxLength(30).IsRequired();
        builder.Property(a => a.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(a => a.AdjustmentDate).HasColumnName("adjustment_date").IsRequired();
        builder.Property(a => a.Reason).HasColumnName("reason").HasMaxLength(200).IsRequired();
        builder.Property(a => a.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(a => a.CreatedBy).HasColumnName("created_by");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.UpdatedBy).HasColumnName("updated_by");
        builder.Property(a => a.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(a => a.DeletedAt).HasColumnName("deleted_at");
        builder.Property(a => a.DeletedBy).HasColumnName("deleted_by");

        builder.HasOne(a => a.Warehouse)
            .WithMany()
            .HasForeignKey(a => a.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Lines)
            .WithOne(l => l.Adjustment)
            .HasForeignKey(l => l.AdjustmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.TenantId, a.DocumentNumber })
            .IsUnique()
            .HasDatabaseName("ix_inventory_adjustments_tenant_id_document_number");

        builder.HasIndex(a => new { a.TenantId, a.AdjustmentDate })
            .HasDatabaseName("ix_inventory_adjustments_tenant_id_adjustment_date");
    }
}

public sealed class InventoryAdjustmentLineConfiguration : IEntityTypeConfiguration<InventoryAdjustmentLine>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustmentLine> builder)
    {
        builder.ToTable("inventory_adjustment_lines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).HasColumnName("id");
        builder.Property(l => l.AdjustmentId).HasColumnName("adjustment_id").IsRequired();
        builder.Property(l => l.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(l => l.QuantityBefore).HasColumnName("quantity_before").HasPrecision(18, 4).IsRequired();
        builder.Property(l => l.QuantityAfter).HasColumnName("quantity_after").HasPrecision(18, 4).IsRequired();
        builder.Property(l => l.LineNumber).HasColumnName("line_number").IsRequired();

        builder.HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.AdjustmentId, l.LineNumber })
            .IsUnique()
            .HasDatabaseName("ix_inventory_adjustment_lines_adjustment_id_line_number");
    }
}
