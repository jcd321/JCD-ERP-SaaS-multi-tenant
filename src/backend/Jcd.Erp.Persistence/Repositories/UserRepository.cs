using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Domain.Tenancy;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByEmailForLoginAsync(
        string email,
        string? tenantSlug,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var query = _context.Users
            .IgnoreQueryFilters()
            .Where(u => u.Email == normalizedEmail && !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(tenantSlug))
        {
            var normalizedSlug = Tenant.NormalizeSlug(tenantSlug);

            return await query
                .Join(
                    _context.Tenants,
                    user => user.TenantId,
                    tenant => tenant.Id,
                    (user, tenant) => new { user, tenant })
                .Where(x => x.tenant.Slug == normalizedSlug && x.tenant.IsActive)
                .Select(x => x.user)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var matches = await query.ToListAsync(cancellationToken);

        return matches.Count switch
        {
            0 => null,
            1 => matches[0],
            _ => null
        };
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _context.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await _context.Users.AddAsync(user, cancellationToken);

    public void Update(User user) => _context.Users.Update(user);
}
