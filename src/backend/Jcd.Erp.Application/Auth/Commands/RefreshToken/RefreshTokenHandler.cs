using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTime;
    private readonly ITenantScope _tenantScope;
    private readonly IUserPermissionService _userPermissions;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IDateTimeService dateTime,
        ITenantScope tenantScope,
        IUserPermissionService userPermissions)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
        _tenantScope = tenantScope;
        _userPermissions = userPermissions;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _jwtTokenService.HashToken(request.RefreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            return Result.Failure<RefreshTokenResponse>("Auth.InvalidRefreshToken");

        _tenantScope.SetTenant(storedToken.TenantId);

        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return Result.Failure<RefreshTokenResponse>("Auth.InvalidRefreshToken");

        var permissions = await _userPermissions.GetPermissionCodesAsync(
            storedToken.TenantId,
            user.Id,
            cancellationToken);
        var accessExpires = _dateTime.UtcNow.AddMinutes(15);
        var refreshExpires = _dateTime.UtcNow.AddDays(7);

        var newRefreshPlain = _jwtTokenService.GenerateRefreshToken();
        var newRefreshHash = _jwtTokenService.HashToken(newRefreshPlain);

        storedToken.Revoke(newRefreshHash);

        await _refreshTokenRepository.AddAsync(
            Domain.Identity.RefreshToken.Create(storedToken.TenantId, user.Id, newRefreshHash, refreshExpires),
            cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id, storedToken.TenantId, user.Email, permissions);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new RefreshTokenResponse(
            accessToken,
            newRefreshPlain,
            accessExpires,
            refreshExpires));
    }
}
