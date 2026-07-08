using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Warehouses;

namespace Jcd.Erp.Domain.Inventory.Stock;

public class StockLevel : BaseAuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public decimal QuantityOnHand { get; private set; }
    public decimal? MinQuantity { get; private set; }
    public decimal? MaxQuantity { get; private set; }

    public Product Product { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;

    private StockLevel() { }

    public static Result<StockLevel> Create(
        Guid tenantId,
        Guid productId,
        Guid warehouseId,
        decimal quantityOnHand,
        decimal? minQuantity = null,
        decimal? maxQuantity = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<StockLevel>("StockLevel.TenantRequired");

        if (productId == Guid.Empty)
            return Result.Failure<StockLevel>("StockLevel.ProductRequired");

        if (warehouseId == Guid.Empty)
            return Result.Failure<StockLevel>("StockLevel.WarehouseRequired");

        if (quantityOnHand < 0)
            return Result.Failure<StockLevel>("StockLevel.QuantityNegative");

        var rangeResult = ValidateQuantityRange(minQuantity, maxQuantity);
        if (rangeResult.IsFailure)
            return Result.Failure<StockLevel>(rangeResult.Error);

        return Result.Success(new StockLevel
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = warehouseId,
            QuantityOnHand = quantityOnHand,
            MinQuantity = minQuantity,
            MaxQuantity = maxQuantity,
            CreatedAt = DateTime.UtcNow,
        });
    }

    public Result UpdateLevels(decimal quantityOnHand, decimal? minQuantity, decimal? maxQuantity)
    {
        if (quantityOnHand < 0)
            return Result.Failure("StockLevel.QuantityNegative");

        var rangeResult = ValidateQuantityRange(minQuantity, maxQuantity);
        if (rangeResult.IsFailure)
            return Result.Failure(rangeResult.Error);

        QuantityOnHand = quantityOnHand;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public bool IsBelowMinimum =>
        MinQuantity.HasValue && QuantityOnHand < MinQuantity.Value;

    private static Result ValidateQuantityRange(decimal? minQuantity, decimal? maxQuantity)
    {
        if (minQuantity is < 0)
            return Result.Failure("StockLevel.MinQuantityNegative");

        if (maxQuantity is < 0)
            return Result.Failure("StockLevel.MaxQuantityNegative");

        if (minQuantity.HasValue && maxQuantity.HasValue && minQuantity.Value > maxQuantity.Value)
            return Result.Failure("StockLevel.MinGreaterThanMax");

        return Result.Success();
    }
}
