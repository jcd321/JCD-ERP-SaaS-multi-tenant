using System.Reflection;
using Jcd.Erp.Domain.Audit;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Configuration;
using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Domain.Tenancy;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Context;

public sealed class ApplicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Must be a property on <see cref="DbContext"/> so EF Core binds the filter per request instance.
  /// Referencing <see cref="ITenantContext"/> directly in expression trees freezes the startup scope instance.
    /// </summary>
    public Guid? CurrentTenantId
    {
        get => _tenantContext.TenantId;
        set => _tenantContext.TenantId = value;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ApplyTenantQueryFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (clrType == typeof(Tenant))
                continue;

            if (typeof(BaseAuditableEntity).IsAssignableFrom(clrType))
            {
                ApplyAuditableTenantFilter(modelBuilder, clrType);
                continue;
            }

            if (typeof(ITenantEntity).IsAssignableFrom(clrType))
            {
                ApplyTenantOnlyFilter(modelBuilder, clrType);
            }
        }
    }

    private void ApplyAuditableTenantFilter(ModelBuilder modelBuilder, Type entityType)
    {
        var method = typeof(ApplicationDbContext)
            .GetMethod(nameof(ConfigureAuditableTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        method.Invoke(this, [modelBuilder]);
    }

    private void ApplyTenantOnlyFilter(ModelBuilder modelBuilder, Type entityType)
    {
        var method = typeof(ApplicationDbContext)
            .GetMethod(nameof(ConfigureTenantOnlyFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        method.Invoke(this, [modelBuilder]);
    }

    private void ConfigureAuditableTenantFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseAuditableEntity =>
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            CurrentTenantId != null && e.TenantId == CurrentTenantId.Value && !e.IsDeleted);

    private void ConfigureTenantOnlyFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantEntity =>
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            CurrentTenantId != null && e.TenantId == CurrentTenantId.Value);
}
