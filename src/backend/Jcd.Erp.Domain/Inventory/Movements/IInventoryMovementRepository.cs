namespace Jcd.Erp.Domain.Inventory.Movements;

public interface IInventoryMovementRepository
{
    Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InventoryMovement> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        Guid? productId,
        string? movementType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default);
}
