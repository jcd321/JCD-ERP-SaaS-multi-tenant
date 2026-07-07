using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Roles.Commands.CreateRole;

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleHandler(
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

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;
        var name = request.Name.Trim();

        if (await _roleRepository.GetByNameAsync(name, cancellationToken) is not null)
            return Result.Failure<Guid>("Role.NameAlreadyExists");

        var roleResult = Role.Create(tenantId, name, request.Description);
        if (roleResult.IsFailure)
            return Result.Failure<Guid>(roleResult.Error);

        var role = roleResult.Value;
        var assignResult = await AssignPermissionsAsync(role, request.PermissionCodes, cancellationToken);
        if (assignResult.IsFailure)
            return Result.Failure<Guid>(assignResult.Error);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(role.Id);
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
