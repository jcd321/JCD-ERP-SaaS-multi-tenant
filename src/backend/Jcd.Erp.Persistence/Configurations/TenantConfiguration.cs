using Jcd.Erp.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(t => t.Slug).HasColumnName("slug").HasMaxLength(100).IsRequired();
        builder.Property(t => t.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(t => t.Slug).IsUnique().HasDatabaseName("ix_tenants_slug");
        builder.HasIndex(t => t.IsActive).HasDatabaseName("ix_tenants_is_active");
    }
}
