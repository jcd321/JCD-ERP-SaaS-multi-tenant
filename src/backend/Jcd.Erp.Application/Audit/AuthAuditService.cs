using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Audit;
using Jcd.Erp.Domain.Identity;

namespace Jcd.Erp.Application.Audit;

public sealed class AuthAuditService : IAuthAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IClientInfoService _clientInfo;
    private readonly IDateTimeService _dateTime;

    public AuthAuditService(
        IAuditLogRepository auditLogRepository,
        IUserSessionRepository userSessionRepository,
        IClientInfoService clientInfo,
        IDateTimeService dateTime)
    {
        _auditLogRepository = auditLogRepository;
        _userSessionRepository = userSessionRepository;
        _clientInfo = clientInfo;
        _dateTime = dateTime;
    }

    public async Task RecordLoginAsync(
        Guid tenantId,
        Guid userId,
        DateTime sessionExpiresAt,
        CancellationToken cancellationToken = default)
    {
        var timestamp = _dateTime.UtcNow;

        await _userSessionRepository.AddAsync(
            UserSession.Create(
                tenantId,
                userId,
                sessionExpiresAt,
                Truncate(_clientInfo.UserAgent, 500),
                Truncate(_clientInfo.IpAddress, 45)),
            cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLog.CreateAuthEvent(
                tenantId,
                userId,
                AuditAction.Login,
                timestamp,
                Truncate(_clientInfo.IpAddress, 45),
                Truncate(_clientInfo.UserAgent, 500)),
            cancellationToken);
    }

    public async Task RecordLogoutAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await _userSessionRepository.RevokeActiveForUserAsync(userId, cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLog.CreateAuthEvent(
                tenantId,
                userId,
                AuditAction.Logout,
                _dateTime.UtcNow,
                Truncate(_clientInfo.IpAddress, 45),
                Truncate(_clientInfo.UserAgent, 500)),
            cancellationToken);
    }

    private static string? Truncate(string? value, int maxLength) =>
        value is null ? null : value.Length <= maxLength ? value : value[..maxLength];
}
