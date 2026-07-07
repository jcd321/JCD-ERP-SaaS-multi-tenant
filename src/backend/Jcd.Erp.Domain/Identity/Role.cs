using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Identity;

public class Role : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = [];
    public ICollection<UserRole> UserRoles { get; private set; } = [];

    private Role() { }

    public static Result<Role> Create(Guid tenantId, string name, string? description = null, bool isSystem = false)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<Role>("Role.TenantRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Role>("Role.NameRequired");

        return Result.Success(new Role
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Description = description?.Trim(),
            IsSystem = isSystem,
            CreatedAt = DateTime.UtcNow
        });
    }

    public void Update(string name, string? description)
    {
        if (IsSystem)
            return;

        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
