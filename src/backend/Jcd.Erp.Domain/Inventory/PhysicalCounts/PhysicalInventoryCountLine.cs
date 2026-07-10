using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Inventory.PhysicalCounts;

public class PhysicalInventoryCountLine
{
    public Guid Id { get; private set; }
    public Guid CountId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal SystemQuantity { get; private set; }
    public decimal? CountedQuantity { get; private set; }
    public int LineNumber { get; private set; }

    public PhysicalInventoryCount Count { get; private set; } = null!;
    public Catalog.Products.Product Product { get; private set; } = null!;

    public bool HasVariance =>
        CountedQuantity.HasValue && CountedQuantity.Value != SystemQuantity;

    public decimal? VarianceDelta =>
        CountedQuantity.HasValue ? CountedQuantity.Value - SystemQuantity : null;

    private PhysicalInventoryCountLine() { }

    internal static Result<PhysicalInventoryCountLine> Create(
        Guid productId,
        decimal systemQuantity,
        int lineNumber)
    {
        if (productId == Guid.Empty)
            return Result.Failure<PhysicalInventoryCountLine>("PhysicalCount.ProductRequired");

        if (systemQuantity < 0)
            return Result.Failure<PhysicalInventoryCountLine>("PhysicalCount.QuantityInvalid");

        if (lineNumber < 1)
            return Result.Failure<PhysicalInventoryCountLine>("PhysicalCount.LineNumberInvalid");

        return Result.Success(new PhysicalInventoryCountLine
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            SystemQuantity = systemQuantity,
            LineNumber = lineNumber,
        });
    }

    internal void AssignCount(Guid countId) => CountId = countId;

    internal Result SetCountedQuantity(decimal? countedQuantity)
    {
        if (countedQuantity.HasValue && countedQuantity.Value < 0)
            return Result.Failure("PhysicalCount.QuantityInvalid");

        CountedQuantity = countedQuantity;
        return Result.Success();
    }
}
