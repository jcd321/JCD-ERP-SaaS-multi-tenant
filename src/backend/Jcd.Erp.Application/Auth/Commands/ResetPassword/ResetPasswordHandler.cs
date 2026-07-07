using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantScope _tenantScope;

    public ResetPasswordHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        ITenantScope tenantScope)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _tenantScope = tenantScope;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var tokenHash = _jwtTokenService.HashToken(request.Token.Trim());

        var resetToken = await _passwordResetTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (resetToken is null || !resetToken.IsActive)
            return Result.Failure("Auth.InvalidResetToken");

        _tenantScope.SetTenant(resetToken.TenantId);

        var user = await _userRepository.GetByIdAsync(resetToken.UserId, cancellationToken);
        if (user is null || !user.IsActive || !string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            return Result.Failure("Auth.InvalidResetToken");

        user.ChangePassword(_passwordHasher.Hash(request.NewPassword));
        _userRepository.Update(user);

        resetToken.MarkUsed();
        _passwordResetTokenRepository.Update(resetToken);

        await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
