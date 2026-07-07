using System.Linq.Expressions;
using Jcd.Erp.Domain.Audit;
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

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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
        var parameter = Expression.Parameter(entityType, "e");
        var tenantIdProperty = Expression.Property(parameter, nameof(BaseAuditableEntity.TenantId));
        var isDeletedProperty = Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));

        var contextTenantId = Expression.Property(
            Expression.Constant(_tenantContext),
            nameof(ITenantContext.TenantId));

        var hasTenant = Expression.NotEqual(contextTenantId, Expression.Constant(null, typeof(Guid?)));
        var tenantMatches = Expression.Equal(tenantIdProperty, Expression.Convert(contextTenantId, typeof(Guid)));
        var notDeleted = Expression.Not(isDeletedProperty);

        var filterBody = Expression.AndAlso(
            hasTenant,
            Expression.AndAlso(tenantMatches, notDeleted));

        var lambda = Expression.Lambda(filterBody, parameter);
        modelBuilder.Entity(entityType).HasQueryFilter(lambda);
    }

    private void ApplyTenantOnlyFilter(ModelBuilder modelBuilder, Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var tenantIdProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));

        var contextTenantId = Expression.Property(
            Expression.Constant(_tenantContext),
            nameof(ITenantContext.TenantId));

        var hasTenant = Expression.NotEqual(contextTenantId, Expression.Constant(null, typeof(Guid?)));
        var tenantMatches = Expression.Equal(tenantIdProperty, Expression.Convert(contextTenantId, typeof(Guid)));

        var filterBody = Expression.AndAlso(hasTenant, tenantMatches);
        var lambda = Expression.Lambda(filterBody, parameter);
        modelBuilder.Entity(entityType).HasQueryFilter(lambda);
    }
}
