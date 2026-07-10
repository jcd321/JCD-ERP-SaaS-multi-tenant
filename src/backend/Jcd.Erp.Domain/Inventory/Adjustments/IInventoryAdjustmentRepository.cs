namespace Jcd.Erp.Domain.Inventory.Adjustments;

public interface IInventoryAdjustmentRepository
{
    Task<InventoryAdjustment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InventoryAdjustment> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(InventoryAdjustment adjustment, CancellationToken cancellationToken = default);
}
