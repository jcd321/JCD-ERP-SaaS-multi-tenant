using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Warehouses;

namespace Jcd.Erp.Domain.Inventory.Transfers;

public class InventoryTransfer : BaseAuditableEntity
{
    public string DocumentNumber { get; private set; } = string.Empty;
    public Guid SourceWarehouseId { get; private set; }
    public Guid DestinationWarehouseId { get; private set; }
    public DateTime TransferDate { get; private set; }
    public string? Notes { get; private set; }

    public Warehouse SourceWarehouse { get; private set; } = null!;
    public Warehouse DestinationWarehouse { get; private set; } = null!;
    public ICollection<InventoryTransferLine> Lines { get; private set; } = [];

    private InventoryTransfer() { }

    public static Result<InventoryTransfer> Create(
        Guid tenantId,
        string documentNumber,
        Guid sourceWarehouseId,
        Guid destinationWarehouseId,
        DateTime transferDate,
        IReadOnlyList<(Guid ProductId, decimal Quantity)> lines,
        string? notes = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<InventoryTransfer>("Transfer.TenantRequired");

        if (string.IsNullOrWhiteSpace(documentNumber))
            return Result.Failure<InventoryTransfer>("Transfer.DocumentNumberRequired");

        if (sourceWarehouseId == Guid.Empty || destinationWarehouseId == Guid.Empty)
            return Result.Failure<InventoryTransfer>("Transfer.WarehouseRequired");

        if (sourceWarehouseId == destinationWarehouseId)
            return Result.Failure<InventoryTransfer>("Transfer.SameWarehouse");

        if (lines.Count == 0)
            return Result.Failure<InventoryTransfer>("Transfer.LinesRequired");

        var transfer = new InventoryTransfer
        {
            TenantId = tenantId,
            DocumentNumber = documentNumber.Trim().ToUpperInvariant(),
            SourceWarehouseId = sourceWarehouseId,
            DestinationWarehouseId = destinationWarehouseId,
            TransferDate = transferDate,
            Notes = NormalizeOptional(notes),
            CreatedAt = DateTime.UtcNow,
        };

        var lineNumber = 1;
        foreach (var (productId, quantity) in lines)
        {
            var lineResult = InventoryTransferLine.Create(productId, quantity, lineNumber);
            if (lineResult.IsFailure)
                return Result.Failure<InventoryTransfer>(lineResult.Error);

            lineResult.Value.AssignTransfer(transfer.Id);
            transfer.Lines.Add(lineResult.Value);
            lineNumber++;
        }

        return Result.Success(transfer);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
