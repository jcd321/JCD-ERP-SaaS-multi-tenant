using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jcd.Erp.Persistence.Seeding;

public sealed class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var catalogPermissions = PermissionCatalog.CreatePermissions().ToList();
        var existingCodes = await _context.Permissions
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        var missing = catalogPermissions
            .Where(p => !existingCodes.Contains(p.Code))
            .ToList();

        if (missing.Count == 0)
        {
            _logger.LogDebug("Permissions catalog is up to date.");
            return;
        }

        await _context.Permissions.AddRangeAsync(missing, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} new permissions from PermissionCatalog.", missing.Count);

        var systemRoles = await _context.Roles
            .IgnoreQueryFilters()
            .Where(r => r.IsSystem && !r.IsDeleted)
            .Include(r => r.RolePermissions)
            .ToListAsync(cancellationToken);

        foreach (var role in systemRoles)
        {
            var assignedIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();

            foreach (var permission in missing)
            {
                if (assignedIds.Contains(permission.Id))
                    continue;

                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id,
                });
            }
        }

        if (systemRoles.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Assigned new permissions to {Count} system administrator role(s).",
                systemRoles.Count);
        }
    }
}
