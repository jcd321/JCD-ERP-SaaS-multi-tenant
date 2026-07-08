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
        ("units", "view", "View units of measure"),
        ("units", "create", "Create units of measure"),
        ("units", "update", "Update units of measure"),
        ("units", "delete", "Delete units of measure"),
        ("categories", "view", "View product categories"),
        ("categories", "create", "Create product categories"),
        ("categories", "update", "Update product categories"),
        ("categories", "delete", "Delete product categories"),
        ("brands", "view", "View brands"),
        ("brands", "create", "Create brands"),
        ("brands", "update", "Update brands"),
        ("brands", "delete", "Delete brands"),
        ("products", "view", "View products"),
        ("products", "create", "Create products"),
        ("products", "update", "Update products"),
        ("products", "delete", "Delete products"),
        ("customers", "view", "View customers"),
        ("customers", "create", "Create customers"),
        ("customers", "update", "Update customers"),
        ("customers", "delete", "Delete customers"),
        ("suppliers", "view", "View suppliers"),
        ("suppliers", "create", "Create suppliers"),
        ("suppliers", "update", "Update suppliers"),
        ("suppliers", "delete", "Delete suppliers"),
    ];

    public static IEnumerable<Permission> CreatePermissions() =>
        All.Select(p => Permission.Create(p.Module, p.Action, p.Description));
}
