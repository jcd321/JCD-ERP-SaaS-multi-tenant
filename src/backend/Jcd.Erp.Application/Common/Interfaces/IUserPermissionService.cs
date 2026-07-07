namespace Jcd.Erp.Application.Common.Interfaces;

public interface IUserPermissionService
{
    Task<IReadOnlyList<string>> GetPermissionCodesAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task InvalidateUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);

    Task InvalidateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
