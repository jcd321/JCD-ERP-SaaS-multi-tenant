using Jcd.Erp.Domain.Identity;

namespace Jcd.Erp.Domain.Identity;

public interface IPermissionRepository
{
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Permission> permissions, CancellationToken cancellationToken = default);
}
