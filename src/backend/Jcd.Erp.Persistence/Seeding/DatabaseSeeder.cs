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
        if (await _context.Permissions.AnyAsync(cancellationToken))
        {
            _logger.LogDebug("Permissions already seeded, skipping.");
            return;
        }

        var permissions = PermissionCatalog.CreatePermissions().ToList();
        await _context.Permissions.AddRangeAsync(permissions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} permissions from PermissionCatalog.", permissions.Count);
    }
}
