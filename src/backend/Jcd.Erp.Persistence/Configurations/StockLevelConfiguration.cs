using Jcd.Erp.Domain.Inventory.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
{
    public void Configure(EntityTypeBuilder<StockLevel> builder)
    {
        builder.ToTable("stock_levels");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(s => s.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(s => s.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(s => s.QuantityOnHand).HasColumnName("quantity_on_hand").HasPrecision(18, 4).IsRequired();
        builder.Property(s => s.MinQuantity).HasColumnName("min_quantity").HasPrecision(18, 4);
        builder.Property(s => s.MaxQuantity).HasColumnName("max_quantity").HasPrecision(18, 4);
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.CreatedBy).HasColumnName("created_by");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");
        builder.Property(s => s.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(s => s.DeletedAt).HasColumnName("deleted_at");
        builder.Property(s => s.DeletedBy).HasColumnName("deleted_by");

        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Warehouse)
            .WithMany()
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.TenantId, s.ProductId, s.WarehouseId })
            .IsUnique()
            .HasDatabaseName("ix_stock_levels_tenant_id_product_id_warehouse_id");

        builder.HasIndex(s => new { s.TenantId, s.WarehouseId, s.IsDeleted })
            .HasDatabaseName("ix_stock_levels_tenant_id_warehouse_id_is_deleted");

        builder.HasIndex(s => new { s.TenantId, s.IsDeleted })
            .HasDatabaseName("ix_stock_levels_tenant_id_is_deleted");
    }
}
