using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Warehouses;

namespace Jcd.Erp.Domain.Inventory.PhysicalCounts;

public class PhysicalInventoryCount : BaseAuditableEntity
{
    public string DocumentNumber { get; private set; } = string.Empty;
    public Guid WarehouseId { get; private set; }
    public DateTime CountDate { get; private set; }
    public string Status { get; private set; } = PhysicalInventoryCountStatuses.Draft;
    public string? Notes { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public Warehouse Warehouse { get; private set; } = null!;
    public ICollection<PhysicalInventoryCountLine> Lines { get; private set; } = [];

    private PhysicalInventoryCount() { }

    public static Result<PhysicalInventoryCount> Create(
        Guid tenantId,
        string documentNumber,
        Guid warehouseId,
        DateTime countDate,
        IReadOnlyList<(Guid ProductId, decimal SystemQuantity)> lines,
        string? notes = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<PhysicalInventoryCount>("PhysicalCount.TenantRequired");

        if (string.IsNullOrWhiteSpace(documentNumber))
            return Result.Failure<PhysicalInventoryCount>("PhysicalCount.DocumentNumberRequired");

        if (warehouseId == Guid.Empty)
            return Result.Failure<PhysicalInventoryCount>("PhysicalCount.WarehouseRequired");

        if (lines.Count == 0)
            return Result.Failure<PhysicalInventoryCount>("PhysicalCount.NoStockInWarehouse");

        var count = new PhysicalInventoryCount
        {
            TenantId = tenantId,
            DocumentNumber = documentNumber.Trim().ToUpperInvariant(),
            WarehouseId = warehouseId,
            CountDate = countDate,
            Notes = NormalizeOptional(notes),
            CreatedAt = DateTime.UtcNow,
        };

        var lineNumber = 1;
        foreach (var (productId, systemQuantity) in lines)
        {
            var lineResult = PhysicalInventoryCountLine.Create(productId, systemQuantity, lineNumber);
            if (lineResult.IsFailure)
                return Result.Failure<PhysicalInventoryCount>(lineResult.Error);

            lineResult.Value.AssignCount(count.Id);
            count.Lines.Add(lineResult.Value);
            lineNumber++;
        }

        return Result.Success(count);
    }

    public Result UpdateLineCounts(IReadOnlyList<(Guid LineId, decimal? CountedQuantity)> updates)
    {
        if (Status != PhysicalInventoryCountStatuses.Draft)
            return Result.Failure("PhysicalCount.NotEditable");

        foreach (var (lineId, countedQuantity) in updates)
        {
            var line = Lines.FirstOrDefault(l => l.Id == lineId);
            if (line is null)
                return Result.Failure("PhysicalCount.LineNotFound");

            var updateResult = line.SetCountedQuantity(countedQuantity);
            if (updateResult.IsFailure)
                return updateResult;
        }

        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != PhysicalInventoryCountStatuses.Draft)
            return Result.Failure("PhysicalCount.NotEditable");

        if (!Lines.Any(l => l.CountedQuantity.HasValue))
            return Result.Failure("PhysicalCount.NoCountedLines");

        Status = PhysicalInventoryCountStatuses.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != PhysicalInventoryCountStatuses.Draft)
            return Result.Failure("PhysicalCount.NotEditable");

        Status = PhysicalInventoryCountStatuses.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
