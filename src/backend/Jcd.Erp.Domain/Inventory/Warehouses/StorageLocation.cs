using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Inventory.Warehouses;

public class StorageLocation : BaseAuditableEntity
{
    public Guid WarehouseId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public string? LocationType { get; private set; }
    public bool IsActive { get; private set; }

    public Warehouse Warehouse { get; private set; } = null!;
    public StorageLocation? Parent { get; private set; }
    public ICollection<StorageLocation> Children { get; private set; } = [];

    private StorageLocation() { }

    public static Result<StorageLocation> Create(
        Guid tenantId,
        Guid warehouseId,
        string code,
        string name,
        string? description = null,
        Guid? parentId = null,
        string? locationType = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<StorageLocation>("Location.TenantRequired");

        if (warehouseId == Guid.Empty)
            return Result.Failure<StorageLocation>("Location.WarehouseRequired");

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<StorageLocation>("Location.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<StorageLocation>("Location.NameRequired");

        return Result.Success(new StorageLocation
        {
            TenantId = tenantId,
            WarehouseId = warehouseId,
            Code = NormalizeCode(code),
            Name = name.Trim(),
            Description = NormalizeOptional(description),
            ParentId = parentId,
            LocationType = NormalizeLocationType(locationType),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result Update(
        string code,
        string name,
        string? description,
        Guid? parentId,
        string? locationType,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure("Location.CodeRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Location.NameRequired");

        if (parentId == Id)
            return Result.Failure("Location.CannotBeOwnParent");

        Code = NormalizeCode(code);
        Name = name.Trim();
        Description = NormalizeOptional(description);
        ParentId = parentId;
        LocationType = NormalizeLocationType(locationType);
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeLocationType(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
