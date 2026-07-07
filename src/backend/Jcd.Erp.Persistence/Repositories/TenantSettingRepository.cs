using Jcd.Erp.Domain.Configuration;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class TenantSettingRepository : ITenantSettingRepository
{
    private readonly ApplicationDbContext _context;

    public TenantSettingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TenantSetting>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.TenantSettings
            .OrderBy(ts => ts.Key)
            .ToListAsync(cancellationToken);

    public Task<TenantSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var normalizedKey = key.Trim().ToLowerInvariant();
        return _context.TenantSettings.FirstOrDefaultAsync(ts => ts.Key == normalizedKey, cancellationToken);
    }

    public async Task AddAsync(TenantSetting setting, CancellationToken cancellationToken = default) =>
        await _context.TenantSettings.AddAsync(setting, cancellationToken);

    public void Update(TenantSetting setting) => _context.TenantSettings.Update(setting);
}
