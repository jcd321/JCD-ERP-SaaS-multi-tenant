using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Catalog.Categories;

public class ProductCategory : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public bool IsActive { get; private set; }

    public ProductCategory? Parent { get; private set; }
    public ICollection<ProductCategory> Children { get; private set; } = [];

    private ProductCategory() { }

    public static Result<ProductCategory> Create(
        Guid tenantId,
        string name,
        string? description = null,
        Guid? parentId = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<ProductCategory>("Category.TenantRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<ProductCategory>("Category.NameRequired");

        return Result.Success(new ProductCategory
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ParentId = parentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result Update(string name, string? description, Guid? parentId, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Category.NameRequired");

        if (parentId == Id)
            return Result.Failure("Category.CannotBeOwnParent");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ParentId = parentId;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
