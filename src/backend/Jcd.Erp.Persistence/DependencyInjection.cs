using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Audit;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Configuration;
using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Domain.Partners.Customers;
using Jcd.Erp.Domain.Partners.Suppliers;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Tenancy;
using Jcd.Erp.Persistence.Context;
using Jcd.Erp.Persistence.Interceptors;
using Jcd.Erp.Persistence.Repositories;
using Jcd.Erp.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jcd.Erp.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<TenantProvider>();
        services.AddScoped<ITenantScope, TenantScope>();
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' is not configured.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<ITenantSettingRepository, TenantSettingRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IStorageLocationRepository, StorageLocationRepository>();
        services.AddScoped<IStockLevelRepository, StockLevelRepository>();

        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
