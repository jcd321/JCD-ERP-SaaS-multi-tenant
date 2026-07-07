using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Users.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserPermissionService _userPermissions;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork,
        IUserPermissionService userPermissions)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
        _userPermissions = userPermissions;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;
        var user = await _userRepository.GetByIdWithRolesAsync(request.Id, cancellationToken);

        if (user is null)
            return Result.Failure("User.NotFound");

        user.UpdateProfile(request.FirstName, request.LastName);

        if (request.IsActive)
            user.Activate();
        else
            user.Deactivate();

        user.UserRoles.Clear();

        if (request.RoleIds.Count > 0)
        {
            foreach (var roleId in request.RoleIds.Distinct())
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
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _userPermissions.InvalidateUserAsync(tenantId, user.Id, cancellationToken);

        return Result.Success();
    }
}
