using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Users.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentTenantService tenant,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tenant = tenant;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            return Result.Failure<Guid>("User.EmailAlreadyExists");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var userResult = User.Create(tenantId, email, passwordHash, request.FirstName, request.LastName);

        if (userResult.IsFailure)
            return Result.Failure<Guid>(userResult.Error);

        var user = userResult.Value;
        user.EmailConfirmed = true;

        var roleResult = await AssignRolesAsync(user, tenantId, request.RoleIds, cancellationToken);
        if (roleResult.IsFailure)
            return Result.Failure<Guid>(roleResult.Error);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }

    private async Task<Result> AssignRolesAsync(
        User user,
        Guid tenantId,
        IReadOnlyList<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        if (roleIds.Count == 0)
            return Result.Success();

        foreach (var roleId in roleIds.Distinct())
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role is null)
                return Result.Failure("Role.NotFound");

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                TenantId = tenantId
            });
        }

        return Result.Success();
    }
}
