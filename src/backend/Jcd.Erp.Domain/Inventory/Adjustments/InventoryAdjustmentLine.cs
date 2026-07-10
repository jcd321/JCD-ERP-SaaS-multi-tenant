using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Inventory.Adjustments;

public class InventoryAdjustmentLine
{
    public Guid Id { get; private set; }
    public Guid AdjustmentId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal QuantityBefore { get; private set; }
    public decimal QuantityAfter { get; private set; }
    public int LineNumber { get; private set; }

    public InventoryAdjustment Adjustment { get; private set; } = null!;
    public Catalog.Products.Product Product { get; private set; } = null!;

    public decimal QuantityDelta => QuantityAfter - QuantityBefore;

    private InventoryAdjustmentLine() { }

    internal static Result<InventoryAdjustmentLine> Create(
        Guid productId,
        decimal quantityBefore,
        decimal countedQuantity,
        int lineNumber)
    {
        if (productId == Guid.Empty)
            return Result.Failure<InventoryAdjustmentLine>("Adjustment.ProductRequired");

        if (quantityBefore < 0 || countedQuantity < 0)
            return Result.Failure<InventoryAdjustmentLine>("Adjustment.QuantityInvalid");

        if (quantityBefore == countedQuantity)
            return Result.Failure<InventoryAdjustmentLine>("Adjustment.NoChange");

        if (lineNumber < 1)
            return Result.Failure<InventoryAdjustmentLine>("Adjustment.LineNumberInvalid");

        return Result.Success(new InventoryAdjustmentLine
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            QuantityBefore = quantityBefore,
            QuantityAfter = countedQuantity,
            LineNumber = lineNumber,
        });
    }

    internal void AssignAdjustment(Guid adjustmentId) => AdjustmentId = adjustmentId;
}
