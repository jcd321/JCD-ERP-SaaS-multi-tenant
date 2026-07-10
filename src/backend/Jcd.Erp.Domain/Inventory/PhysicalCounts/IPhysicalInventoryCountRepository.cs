namespace Jcd.Erp.Domain.Inventory.PhysicalCounts;

public interface IPhysicalInventoryCountRepository
{
    Task<PhysicalInventoryCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PhysicalInventoryCount> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(PhysicalInventoryCount count, CancellationToken cancellationToken = default);

    void Update(PhysicalInventoryCount count);
}
