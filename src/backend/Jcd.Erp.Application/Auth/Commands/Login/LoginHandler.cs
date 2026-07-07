using Jcd.Erp.Application.Audit;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Domain.Tenancy;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTime;
    private readonly ITenantScope _tenantScope;
    private readonly IAuthAuditService _authAuditService;

    public LoginHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IDateTimeService dateTime,
        ITenantScope tenantScope,
        IAuthAuditService authAuditService)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _roleRepository = roleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
        _tenantScope = tenantScope;
        _authAuditService = authAuditService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailForLoginAsync(email, request.TenantSlug, cancellationToken);

        if (user is null)
        {
            if (string.IsNullOrWhiteSpace(request.TenantSlug))
            {
                var matches = await _userRepository.GetLoginMatchesByEmailAsync(email, cancellationToken);
                if (matches.Count > 1)
                    return Result.Failure<LoginResponse>("Auth.TenantSlugRequired");
            }

            return Result.Failure<LoginResponse>("Auth.InvalidCredentials");
        }

        if (!user.IsActive)
            return Result.Failure<LoginResponse>("Auth.InvalidCredentials");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>("Auth.InvalidCredentials");

        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant is null || !tenant.IsActive)
            return Result.Failure<LoginResponse>("Auth.TenantInactive");

        _tenantScope.SetTenant(tenant.Id);

        var permissions = await _roleRepository.GetUserPermissionCodesAsync(user.Id, cancellationToken);
        var accessExpires = _dateTime.UtcNow.AddMinutes(15);
        var refreshExpires = request.RememberMe
            ? _dateTime.UtcNow.AddDays(30)
            : _dateTime.UtcNow.AddDays(7);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id, tenant.Id, user.Email, permissions);

        var refreshTokenPlain = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = _jwtTokenService.HashToken(refreshTokenPlain);

        await _refreshTokenRepository.AddAsync(
            Domain.Identity.RefreshToken.Create(tenant.Id, user.Id, refreshTokenHash, refreshExpires),
            cancellationToken);

        await _authAuditService.RecordLoginAsync(tenant.Id, user.Id, refreshExpires, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginResponse(
            tenant.Id,
            tenant.Slug,
            user.Id,
            user.Email,
            user.FullName,
            permissions,
            accessToken,
            refreshTokenPlain,
            accessExpires,
            refreshExpires));
    }
}
