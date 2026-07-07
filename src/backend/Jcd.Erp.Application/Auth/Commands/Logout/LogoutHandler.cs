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

    public LogoutHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUserService currentUser,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var hash = _jwtTokenService.HashToken(request.RefreshToken);
            var token = await _refreshTokenRepository.GetByTokenHashAsync(hash, cancellationToken);
            token?.Revoke();
        }
        else if (_currentUser.UserId.HasValue)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(_currentUser.UserId.Value, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
