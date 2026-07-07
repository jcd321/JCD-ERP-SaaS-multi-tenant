using Jcd.Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jcd.Erp.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
        builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed").IsRequired();
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(u => u.CreatedBy).HasColumnName("created_by");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        builder.Property(u => u.UpdatedBy).HasColumnName("updated_by");
        builder.Property(u => u.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at");
        builder.Property(u => u.DeletedBy).HasColumnName("deleted_by");

        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique()
            .HasDatabaseName("ix_users_tenant_id_email");

        builder.HasIndex(u => new { u.TenantId, u.IsActive })
            .HasDatabaseName("ix_users_tenant_id_is_active");

        builder.HasIndex(u => new { u.TenantId, u.IsDeleted })
            .HasDatabaseName("ix_users_tenant_id_is_deleted");

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
