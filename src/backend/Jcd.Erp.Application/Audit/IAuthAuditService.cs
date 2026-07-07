namespace Jcd.Erp.Application.Audit;

public interface IAuthAuditService
{
    Task RecordLoginAsync(
        Guid tenantId,
        Guid userId,
        DateTime sessionExpiresAt,
        CancellationToken cancellationToken = default);

    Task RecordLogoutAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
