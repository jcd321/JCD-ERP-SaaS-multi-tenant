using Jcd.Erp.Domain.Tenancy;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Tenants.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = Tenant.NormalizeSlug(slug);
        return _context.Tenants.FirstOrDefaultAsync(t => t.Slug == normalizedSlug, cancellationToken);
    }

    public Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = Tenant.NormalizeSlug(slug);
        return _context.Tenants.AnyAsync(t => t.Slug == normalizedSlug, cancellationToken);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default) =>
        await _context.Tenants.AddAsync(tenant, cancellationToken);
}
