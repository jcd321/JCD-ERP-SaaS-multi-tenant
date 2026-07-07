using Jcd.Erp.Application.Common.Cache;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Identity;

namespace Jcd.Erp.Application.Common.Services;

public sealed class UserPermissionService : IUserPermissionService
{
    private static readonly TimeSpan PermissionsTtl = TimeSpan.FromMinutes(30);

    private readonly IRoleRepository _roleRepository;
    private readonly ICacheService _cache;

    public UserPermissionService(IRoleRepository roleRepository, ICacheService cache)
    {
        _roleRepository = roleRepository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<string>> GetPermissionCodesAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.UserPermissions(tenantId, userId);
        var cached = await _cache.GetAsync<List<string>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var permissions = await _roleRepository.GetUserPermissionCodesAsync(userId, cancellationToken);
        var cachedPermissions = permissions.ToList();
        await _cache.SetAsync(cacheKey, cachedPermissions, PermissionsTtl, cancellationToken);
        return cachedPermissions;
    }

    public Task InvalidateUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(CacheKeys.UserPermissions(tenantId, userId), cancellationToken);

    public Task InvalidateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _cache.RemoveByPrefixAsync(CacheKeys.TenantPermissionsPrefix(tenantId), cancellationToken);
}
