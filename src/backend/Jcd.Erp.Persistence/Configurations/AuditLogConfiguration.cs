using Jcd.Erp.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.Id).HasColumnName("id");
        builder.Property(al => al.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(al => al.UserId).HasColumnName("user_id");
        builder.Property(al => al.Action).HasColumnName("action").IsRequired();
        builder.Property(al => al.EntityName).HasColumnName("entity_name").HasMaxLength(100).IsRequired();
        builder.Property(al => al.EntityId).HasColumnName("entity_id");
        builder.Property(al => al.OldValues).HasColumnName("old_values");
        builder.Property(al => al.NewValues).HasColumnName("new_values");
        builder.Property(al => al.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
        builder.Property(al => al.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
        builder.Property(al => al.Timestamp).HasColumnName("timestamp").IsRequired();

        builder.HasIndex(al => new { al.TenantId, al.Timestamp })
            .HasDatabaseName("ix_audit_logs_tenant_id_timestamp");

        builder.HasIndex(al => new { al.TenantId, al.UserId })
            .HasDatabaseName("ix_audit_logs_tenant_id_user_id");

        builder.HasIndex(al => new { al.TenantId, al.EntityName, al.EntityId })
            .HasDatabaseName("ix_audit_logs_tenant_id_entity_name_entity_id");
    }
}
