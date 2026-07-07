namespace Jcd.Erp.Application.Common.Cache;

public static class CacheKeys
{
    public static string TenantSettings(Guid tenantId) => $"tenant:{tenantId}:settings:all";

    public static string UserPermissions(Guid tenantId, Guid userId) =>
        $"tenant:{tenantId}:permissions:user:{userId}";

    public static string TenantPermissionsPrefix(Guid tenantId) => $"tenant:{tenantId}:permissions:";

    public static string TenantSettingsPrefix(Guid tenantId) => $"tenant:{tenantId}:settings:";
}
