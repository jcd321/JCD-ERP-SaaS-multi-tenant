using Jcd.Erp.Domain.Inventory.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class InventoryTransferConfiguration : IEntityTypeConfiguration<InventoryTransfer>
{
    public void Configure(EntityTypeBuilder<InventoryTransfer> builder)
    {
        builder.ToTable("inventory_transfers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(t => t.DocumentNumber).HasColumnName("document_number").HasMaxLength(30).IsRequired();
        builder.Property(t => t.SourceWarehouseId).HasColumnName("source_warehouse_id").IsRequired();
        builder.Property(t => t.DestinationWarehouseId).HasColumnName("destination_warehouse_id").IsRequired();
        builder.Property(t => t.TransferDate).HasColumnName("transfer_date").IsRequired();
        builder.Property(t => t.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(t => t.CreatedBy).HasColumnName("created_by");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");
        builder.Property(t => t.UpdatedBy).HasColumnName("updated_by");
        builder.Property(t => t.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(t => t.DeletedAt).HasColumnName("deleted_at");
        builder.Property(t => t.DeletedBy).HasColumnName("deleted_by");

        builder.HasOne(t => t.SourceWarehouse)
            .WithMany()
            .HasForeignKey(t => t.SourceWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.DestinationWarehouse)
            .WithMany()
            .HasForeignKey(t => t.DestinationWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Lines)
            .WithOne(l => l.Transfer)
            .HasForeignKey(l => l.TransferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.TenantId, t.DocumentNumber })
            .IsUnique()
            .HasDatabaseName("ix_inventory_transfers_tenant_id_document_number");

        builder.HasIndex(t => new { t.TenantId, t.TransferDate })
            .HasDatabaseName("ix_inventory_transfers_tenant_id_transfer_date");
    }
}

public sealed class InventoryTransferLineConfiguration : IEntityTypeConfiguration<InventoryTransferLine>
{
    public void Configure(EntityTypeBuilder<InventoryTransferLine> builder)
    {
        builder.ToTable("inventory_transfer_lines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).HasColumnName("id");
        builder.Property(l => l.TransferId).HasColumnName("transfer_id").IsRequired();
        builder.Property(l => l.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(l => l.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(l => l.LineNumber).HasColumnName("line_number").IsRequired();

        builder.HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.TransferId, l.LineNumber })
            .IsUnique()
            .HasDatabaseName("ix_inventory_transfer_lines_transfer_id_line_number");
    }
}
