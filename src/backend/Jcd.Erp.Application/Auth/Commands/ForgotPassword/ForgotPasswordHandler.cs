using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;
    private readonly IAppSettings _appSettings;
    private readonly IDateTimeService _dateTime;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantScope _tenantScope;

    public ForgotPasswordHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IJwtTokenService jwtTokenService,
        IEmailService emailService,
        IAppSettings appSettings,
        IDateTimeService dateTime,
        IUnitOfWork unitOfWork,
        ITenantScope tenantScope)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
        _appSettings = appSettings;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
        _tenantScope = tenantScope;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailForLoginAsync(email, request.TenantSlug, cancellationToken);

        if (user is null)
        {
            if (string.IsNullOrWhiteSpace(request.TenantSlug))
            {
                var matches = await _userRepository.GetLoginMatchesByEmailAsync(email, cancellationToken);
                if (matches.Count > 1)
                    return Result.Failure("Auth.TenantSlugRequired");
            }

            return Result.Success();
        }

        if (!user.IsActive)
            return Result.Success();

        _tenantScope.SetTenant(user.TenantId);

        await _passwordResetTokenRepository.InvalidateAllForUserAsync(user.Id, cancellationToken);

        var tokenPlain = _jwtTokenService.GenerateRefreshToken();
        var tokenHash = _jwtTokenService.HashToken(tokenPlain);
        var expiresAt = _dateTime.UtcNow.AddMinutes(_appSettings.PasswordResetTokenMinutes);

        await _passwordResetTokenRepository.AddAsync(
            PasswordResetToken.Create(user.TenantId, user.Id, tokenHash, expiresAt),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resetLink =
            $"{_appSettings.FrontendUrl.TrimEnd('/')}/auth/reset-password" +
            $"?token={Uri.EscapeDataString(tokenPlain)}&email={Uri.EscapeDataString(email)}";

        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink, cancellationToken);

        return Result.Success();
    }
}
