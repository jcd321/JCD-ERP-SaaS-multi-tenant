using Jcd.Erp.Domain.Configuration;

namespace Jcd.Erp.Domain.Configuration;

public interface ITenantSettingRepository
{
    Task<IReadOnlyList<TenantSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task AddAsync(TenantSetting setting, CancellationToken cancellationToken = default);
    void Update(TenantSetting setting);
}
