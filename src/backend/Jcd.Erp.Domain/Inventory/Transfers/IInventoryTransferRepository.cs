namespace Jcd.Erp.Domain.Inventory.Transfers;

public interface IInventoryTransferRepository
{
    Task<InventoryTransfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InventoryTransfer> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? sourceWarehouseId,
        Guid? destinationWarehouseId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(InventoryTransfer transfer, CancellationToken cancellationToken = default);
}
