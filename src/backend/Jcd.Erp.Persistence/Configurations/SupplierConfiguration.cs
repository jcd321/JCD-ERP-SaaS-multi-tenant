using Jcd.Erp.Domain.Partners.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(s => s.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.Property(s => s.LegalName).HasColumnName("legal_name").HasMaxLength(200).IsRequired();
        builder.Property(s => s.TradeName).HasColumnName("trade_name").HasMaxLength(200);
        builder.Property(s => s.TaxId).HasColumnName("tax_id").HasMaxLength(30);
        builder.Property(s => s.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(s => s.Phone).HasColumnName("phone").HasMaxLength(30);
        builder.Property(s => s.MobilePhone).HasColumnName("mobile_phone").HasMaxLength(30);
        builder.Property(s => s.AddressLine1).HasColumnName("address_line1").HasMaxLength(300);
        builder.Property(s => s.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(s => s.StateOrProvince).HasColumnName("state_or_province").HasMaxLength(100);
        builder.Property(s => s.CountryCode).HasColumnName("country_code").HasMaxLength(2);
        builder.Property(s => s.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(s => s.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.CreatedBy).HasColumnName("created_by");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");
        builder.Property(s => s.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(s => s.DeletedAt).HasColumnName("deleted_at");
        builder.Property(s => s.DeletedBy).HasColumnName("deleted_by");

        builder.HasIndex(s => new { s.TenantId, s.Code })
            .IsUnique()
            .HasDatabaseName("ix_suppliers_tenant_id_code");

        builder.HasIndex(s => new { s.TenantId, s.TaxId })
            .IsUnique()
            .HasFilter("tax_id IS NOT NULL")
            .HasDatabaseName("ix_suppliers_tenant_id_tax_id");

        builder.HasIndex(s => new { s.TenantId, s.IsDeleted })
            .HasDatabaseName("ix_suppliers_tenant_id_is_deleted");
    }
}
