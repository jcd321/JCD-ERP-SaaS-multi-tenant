using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class PhysicalInventoryCountConfiguration : IEntityTypeConfiguration<PhysicalInventoryCount>
{
    public void Configure(EntityTypeBuilder<PhysicalInventoryCount> builder)
    {
        builder.ToTable("physical_inventory_counts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(c => c.DocumentNumber).HasColumnName("document_number").HasMaxLength(30).IsRequired();
        builder.Property(c => c.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(c => c.CountDate).HasColumnName("count_date").IsRequired();
        builder.Property(c => c.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(c => c.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(c => c.CompletedAt).HasColumnName("completed_at");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(c => c.CreatedBy).HasColumnName("created_by");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");
        builder.Property(c => c.DeletedBy).HasColumnName("deleted_by");

        builder.HasOne(c => c.Warehouse)
            .WithMany()
            .HasForeignKey(c => c.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Lines)
            .WithOne(l => l.Count)
            .HasForeignKey(l => l.CountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.TenantId, c.DocumentNumber })
            .IsUnique()
            .HasDatabaseName("ix_physical_inventory_counts_tenant_id_document_number");

        builder.HasIndex(c => new { c.TenantId, c.CountDate })
            .HasDatabaseName("ix_physical_inventory_counts_tenant_id_count_date");
    }
}

public sealed class PhysicalInventoryCountLineConfiguration : IEntityTypeConfiguration<PhysicalInventoryCountLine>
{
    public void Configure(EntityTypeBuilder<PhysicalInventoryCountLine> builder)
    {
        builder.ToTable("physical_inventory_count_lines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).HasColumnName("id");
        builder.Property(l => l.CountId).HasColumnName("count_id").IsRequired();
        builder.Property(l => l.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(l => l.SystemQuantity).HasColumnName("system_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(l => l.CountedQuantity).HasColumnName("counted_quantity").HasPrecision(18, 4);
        builder.Property(l => l.LineNumber).HasColumnName("line_number").IsRequired();

        builder.HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.CountId, l.LineNumber })
            .IsUnique()
            .HasDatabaseName("ix_physical_inventory_count_lines_count_id_line_number");
    }
}
