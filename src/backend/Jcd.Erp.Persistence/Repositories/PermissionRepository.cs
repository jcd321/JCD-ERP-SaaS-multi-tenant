using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Permissions
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Action)
            .ToListAsync(cancellationToken);

    public Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();
        return _context.Permissions.FirstOrDefaultAsync(p => p.Code == normalizedCode, cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<Permission> permissions,
        CancellationToken cancellationToken = default) =>
        await _context.Permissions.AddRangeAsync(permissions, cancellationToken);
}
