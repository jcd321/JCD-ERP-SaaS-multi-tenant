using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Inventory.Transfers;

public class InventoryTransferLine
{
    public Guid Id { get; private set; }
    public Guid TransferId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public int LineNumber { get; private set; }

    public InventoryTransfer Transfer { get; private set; } = null!;
    public Catalog.Products.Product Product { get; private set; } = null!;

    private InventoryTransferLine() { }

    internal static Result<InventoryTransferLine> Create(
        Guid productId,
        decimal quantity,
        int lineNumber)
    {
        if (productId == Guid.Empty)
            return Result.Failure<InventoryTransferLine>("Transfer.ProductRequired");

        if (quantity <= 0)
            return Result.Failure<InventoryTransferLine>("Transfer.QuantityInvalid");

        if (lineNumber < 1)
            return Result.Failure<InventoryTransferLine>("Transfer.LineNumberInvalid");

        return Result.Success(new InventoryTransferLine
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Quantity = quantity,
            LineNumber = lineNumber,
        });
    }

    internal void AssignTransfer(Guid transferId) => TransferId = transferId;
}
