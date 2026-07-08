namespace Jcd.Erp.Domain.Inventory.Warehouses;

public interface IStorageLocationRepository
{
    Task<StorageLocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StorageLocation?> GetByCodeAsync(
        Guid warehouseId,
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> HasChildrenAsync(Guid locationId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<StorageLocation> Items, int TotalCount)> GetPagedAsync(
        Guid warehouseId,
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StorageLocation>> GetParentOptionsAsync(
        Guid warehouseId,
        Guid? excludeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(StorageLocation location, CancellationToken cancellationToken = default);

    void Update(StorageLocation location);

    void Delete(StorageLocation location);
}
