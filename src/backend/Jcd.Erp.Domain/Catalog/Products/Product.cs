using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Catalog.Products;

public class Product : BaseAuditableEntity
{
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? BrandId { get; private set; }
    public Guid UnitId { get; private set; }
    public bool IsActive { get; private set; }

    public ProductCategory Category { get; private set; } = null!;
    public Brand? Brand { get; private set; }
    public UnitOfMeasure Unit { get; private set; } = null!;

    private Product() { }

    public static Result<Product> Create(
        Guid tenantId,
        string sku,
        string name,
        Guid categoryId,
        Guid unitId,
        string? description = null,
        Guid? brandId = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<Product>("Product.TenantRequired");

        if (string.IsNullOrWhiteSpace(sku))
            return Result.Failure<Product>("Product.SkuRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Product>("Product.NameRequired");

        if (categoryId == Guid.Empty)
            return Result.Failure<Product>("Product.CategoryRequired");

        if (unitId == Guid.Empty)
            return Result.Failure<Product>("Product.UnitRequired");

        return Result.Success(new Product
        {
            TenantId = tenantId,
            Sku = NormalizeSku(sku),
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CategoryId = categoryId,
            BrandId = brandId,
            UnitId = unitId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result Update(
        string sku,
        string name,
        Guid categoryId,
        Guid unitId,
        string? description,
        Guid? brandId,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return Result.Failure("Product.SkuRequired");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Product.NameRequired");

        if (categoryId == Guid.Empty)
            return Result.Failure("Product.CategoryRequired");

        if (unitId == Guid.Empty)
            return Result.Failure("Product.UnitRequired");

        Sku = NormalizeSku(sku);
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CategoryId = categoryId;
        BrandId = brandId;
        UnitId = unitId;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static string NormalizeSku(string sku) => sku.Trim().ToUpperInvariant();
}
