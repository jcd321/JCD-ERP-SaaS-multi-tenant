using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Warehouses;

namespace Jcd.Erp.Domain.Inventory.Adjustments;

public class InventoryAdjustment : BaseAuditableEntity
{
    public string DocumentNumber { get; private set; } = string.Empty;
    public Guid WarehouseId { get; private set; }
    public DateTime AdjustmentDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    public Warehouse Warehouse { get; private set; } = null!;
    public ICollection<InventoryAdjustmentLine> Lines { get; private set; } = [];

    private InventoryAdjustment() { }

    public static Result<InventoryAdjustment> Create(
        Guid tenantId,
        string documentNumber,
        Guid warehouseId,
        DateTime adjustmentDate,
        string reason,
        IReadOnlyList<(Guid ProductId, decimal QuantityBefore, decimal CountedQuantity)> lines,
        string? notes = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<InventoryAdjustment>("Adjustment.TenantRequired");

        if (string.IsNullOrWhiteSpace(documentNumber))
            return Result.Failure<InventoryAdjustment>("Adjustment.DocumentNumberRequired");

        if (warehouseId == Guid.Empty)
            return Result.Failure<InventoryAdjustment>("Adjustment.WarehouseRequired");

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure<InventoryAdjustment>("Adjustment.ReasonRequired");

        if (lines.Count == 0)
            return Result.Failure<InventoryAdjustment>("Adjustment.LinesRequired");

        var adjustment = new InventoryAdjustment
        {
            TenantId = tenantId,
            DocumentNumber = documentNumber.Trim().ToUpperInvariant(),
            WarehouseId = warehouseId,
            AdjustmentDate = adjustmentDate,
            Reason = reason.Trim(),
            Notes = NormalizeOptional(notes),
            CreatedAt = DateTime.UtcNow,
        };

        var lineNumber = 1;
        foreach (var (productId, quantityBefore, countedQuantity) in lines)
        {
            var lineResult = InventoryAdjustmentLine.Create(
                productId,
                quantityBefore,
                countedQuantity,
                lineNumber);

            if (lineResult.IsFailure)
                return Result.Failure<InventoryAdjustment>(lineResult.Error);

            lineResult.Value.AssignAdjustment(adjustment.Id);
            adjustment.Lines.Add(lineResult.Value);
            lineNumber++;
        }

        return Result.Success(adjustment);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
