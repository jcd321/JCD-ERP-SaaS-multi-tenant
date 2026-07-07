using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Roles.Commands.DeleteRole;

public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserPermissionService _userPermissions;

    public DeleteRoleHandler(
        IRoleRepository roleRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork,
        IUserPermissionService userPermissions)
    {
        _roleRepository = roleRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
        _userPermissions = userPermissions;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return Result.Failure("Role.NotFound");

        if (role.IsSystem)
            return Result.Failure("Role.IsSystem");

        if (await _roleRepository.HasAssignedUsersAsync(role.Id, cancellationToken))
            return Result.Failure("Role.HasUsers");

        _roleRepository.Delete(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _userPermissions.InvalidateTenantAsync(_tenant.TenantId, cancellationToken);

        return Result.Success();
    }
}
