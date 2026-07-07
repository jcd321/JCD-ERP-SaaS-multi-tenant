using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;

namespace Jcd.Erp.Application.Roles;

internal static class RolePermissionAssigner
{
    public static async Task<Result> AssignAsync(
        Role role,
        IReadOnlyList<string> permissionCodes,
        IPermissionRepository permissionRepository,
        CancellationToken cancellationToken)
    {
        if (permissionCodes.Count == 0)
            return Result.Success();

        var allPermissions = await permissionRepository.GetAllAsync(cancellationToken);
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
