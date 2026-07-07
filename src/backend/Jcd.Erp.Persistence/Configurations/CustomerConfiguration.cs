using Jcd.Erp.Domain.Partners.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(c => c.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.Property(c => c.LegalName).HasColumnName("legal_name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.TradeName).HasColumnName("trade_name").HasMaxLength(200);
        builder.Property(c => c.TaxId).HasColumnName("tax_id").HasMaxLength(30);
        builder.Property(c => c.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(c => c.Phone).HasColumnName("phone").HasMaxLength(30);
        builder.Property(c => c.MobilePhone).HasColumnName("mobile_phone").HasMaxLength(30);
        builder.Property(c => c.AddressLine1).HasColumnName("address_line1").HasMaxLength(300);
        builder.Property(c => c.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(c => c.StateOrProvince).HasColumnName("state_or_province").HasMaxLength(100);
        builder.Property(c => c.CountryCode).HasColumnName("country_code").HasMaxLength(2);
        builder.Property(c => c.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(c => c.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(c => c.CreatedBy).HasColumnName("created_by");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");
        builder.Property(c => c.DeletedBy).HasColumnName("deleted_by");

        builder.HasIndex(c => new { c.TenantId, c.Code })
            .IsUnique()
            .HasDatabaseName("ix_customers_tenant_id_code");

        builder.HasIndex(c => new { c.TenantId, c.TaxId })
            .IsUnique()
            .HasFilter("tax_id IS NOT NULL")
            .HasDatabaseName("ix_customers_tenant_id_tax_id");

        builder.HasIndex(c => new { c.TenantId, c.IsDeleted })
            .HasDatabaseName("ix_customers_tenant_id_is_deleted");
    }
}
