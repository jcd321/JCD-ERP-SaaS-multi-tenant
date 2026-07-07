using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Identity;

public class Permission : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Module { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = [];

    private Permission() { }

    public static Permission Create(string module, string action, string? description = null)
    {
        var code = $"{module}.{action}".ToLowerInvariant();
        return new Permission
        {
            Code = code,
            Module = module.ToLowerInvariant(),
            Action = action.ToLowerInvariant(),
            Description = description
        };
    }
}
