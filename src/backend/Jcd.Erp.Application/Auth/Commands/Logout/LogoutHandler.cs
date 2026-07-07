using Jcd.Erp.Application.Audit;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthAuditService _authAuditService;
    private readonly ICurrentTenantService _currentTenant;

    public LogoutHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUserService currentUser,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IAuthAuditService authAuditService,
        ICurrentTenantService currentTenant)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _authAuditService = authAuditService;
        _currentTenant = currentTenant;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        Guid? userId = null;
        Guid? tenantId = _currentTenant.HasTenant ? _currentTenant.TenantId : null;

        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var hash = _jwtTokenService.HashToken(request.RefreshToken);
            var token = await _refreshTokenRepository.GetByTokenHashAsync(hash, cancellationToken);
            if (token is not null)
            {
                userId = token.UserId;
                tenantId ??= token.TenantId;
                token.Revoke();
            }
        }
        else if (_currentUser.UserId.HasValue)
        {
            userId = _currentUser.UserId.Value;
            await _refreshTokenRepository.RevokeAllForUserAsync(userId.Value, cancellationToken);
        }

        if (userId.HasValue && tenantId.HasValue)
            await _authAuditService.RecordLogoutAsync(tenantId.Value, userId.Value, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
