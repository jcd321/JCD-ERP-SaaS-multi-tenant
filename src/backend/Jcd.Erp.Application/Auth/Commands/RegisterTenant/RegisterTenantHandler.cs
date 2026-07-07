using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Configuration;
using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Domain.Tenancy;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.RegisterTenant;

public class RegisterTenantHandler : IRequestHandler<RegisterTenantCommand, Result<RegisterTenantResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITenantSettingRepository _tenantSettingRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTime;

    public RegisterTenantHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITenantSettingRepository tenantSettingRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IDateTimeService dateTime)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tenantSettingRepository = tenantSettingRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result<RegisterTenantResponse>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenantResult = Tenant.Create(request.CompanyName, request.Slug);
        if (tenantResult.IsFailure)
            return Result.Failure<RegisterTenantResponse>(tenantResult.Error);

        var tenant = tenantResult.Value;
        if (await _tenantRepository.ExistsBySlugAsync(tenant.Slug, cancellationToken))
            return Result.Failure<RegisterTenantResponse>("Tenant.SlugAlreadyExists");

        var passwordHash = _passwordHasher.Hash(request.AdminPassword);
        var userResult = User.Create(
            tenant.Id,
            request.AdminEmail,
            passwordHash,
            request.AdminFirstName,
            request.AdminLastName);

        if (userResult.IsFailure)
            return Result.Failure<RegisterTenantResponse>(userResult.Error);

        var user = userResult.Value;
        user.EmailConfirmed = true;

        var adminRoleResult = Role.Create(tenant.Id, "Administrator", "Full system access", isSystem: true);
        if (adminRoleResult.IsFailure)
            return Result.Failure<RegisterTenantResponse>(adminRoleResult.Error);

        var adminRole = adminRoleResult.Value;
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        foreach (var permission in permissions)
        {
            adminRole.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id
            });
        }

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = adminRole.Id,
            TenantId = tenant.Id
        });

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _roleRepository.AddAsync(adminRole, cancellationToken);
        await _userRepository.AddAsync(user, cancellationToken);

        await _tenantSettingRepository.AddAsync(
            TenantSetting.Create(tenant.Id, "default_language", "es"),
            cancellationToken);

        await _tenantSettingRepository.AddAsync(
            TenantSetting.Create(tenant.Id, "default_currency", "USD"),
            cancellationToken);

        var permissionCodes = permissions.Select(p => p.Code).ToList();
        var accessExpires = _dateTime.UtcNow.AddMinutes(15);
        var refreshExpires = _dateTime.UtcNow.AddDays(7);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id, tenant.Id, user.Email, permissionCodes);

        var refreshTokenPlain = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = _jwtTokenService.HashToken(refreshTokenPlain);

        await _refreshTokenRepository.AddAsync(
            Domain.Identity.RefreshToken.Create(tenant.Id, user.Id, refreshTokenHash, refreshExpires),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new RegisterTenantResponse(
            tenant.Id,
            tenant.Slug,
            user.Id,
            accessToken,
            refreshTokenPlain,
            accessExpires,
            refreshExpires));
    }
}
