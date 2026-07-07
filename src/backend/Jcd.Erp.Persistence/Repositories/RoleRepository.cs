using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<bool> HasAssignedUsersAsync(Guid roleId, CancellationToken cancellationToken = default) =>
        _context.UserRoles.AnyAsync(ur => ur.RoleId == roleId, cancellationToken);

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        return _context.Roles.FirstOrDefaultAsync(r => r.Name == normalizedName, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<string>> GetUserPermissionCodesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(
                _context.RolePermissions,
                ur => ur.RoleId,
                rp => rp.RoleId,
                (ur, rp) => rp.PermissionId)
            .Join(
                _context.Permissions,
                permissionId => permissionId,
                p => p.Id,
                (_, p) => p.Code)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default) =>
        await _context.Roles.AddAsync(role, cancellationToken);

    public void Update(Role role) => _context.Roles.Update(role);

    public void Delete(Role role)
    {
        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
        _context.Roles.Update(role);
    }
}
