using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Roles.Commands.UpdateRole;

public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var role = await _roleRepository.GetByIdWithPermissionsAsync(request.Id, cancellationToken);
        if (role is null)
            return Result.Failure("Role.NotFound");

        if (role.IsSystem)
            return Result.Failure("Role.IsSystem");

        var name = request.Name.Trim();
        var existing = await _roleRepository.GetByNameAsync(name, cancellationToken);
        if (existing is not null && existing.Id != role.Id)
            return Result.Failure("Role.NameAlreadyExists");

        role.Update(name, request.Description);
        role.RolePermissions.Clear();

        var assignResult = await AssignPermissionsAsync(role, request.PermissionCodes, cancellationToken);
        if (assignResult.IsFailure)
            return assignResult;

        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> AssignPermissionsAsync(
        Role role,
        IReadOnlyList<string> permissionCodes,
        CancellationToken cancellationToken)
    {
        if (permissionCodes.Count == 0)
            return Result.Success();

        var allPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
        var permissionMap = allPermissions.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);

        foreach (var code in permissionCodes.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!permissionMap.TryGetValue(code, out var permission))
                return Result.Failure("Permission.NotFound");

            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });
        }

        return Result.Success();
    }
}
