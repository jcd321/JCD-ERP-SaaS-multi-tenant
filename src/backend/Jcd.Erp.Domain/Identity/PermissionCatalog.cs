using Jcd.Erp.Domain.Configuration;
using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Domain.Tenancy;

namespace Jcd.Erp.Domain.Identity;

public static class PermissionCatalog
{
    public static IReadOnlyList<(string Module, string Action, string Description)> All { get; } =
    [
        ("users", "view", "View users"),
        ("users", "create", "Create users"),
        ("users", "update", "Update users"),
        ("users", "delete", "Delete users"),
        ("roles", "view", "View roles"),
        ("roles", "create", "Create roles"),
        ("roles", "update", "Update roles"),
        ("roles", "delete", "Delete roles"),
        ("settings", "view", "View tenant settings"),
        ("settings", "update", "Update tenant settings"),
        ("audit", "view", "View audit logs"),
    ];

    public static IEnumerable<Permission> CreatePermissions() =>
        All.Select(p => Permission.Create(p.Module, p.Action, p.Description));
}
