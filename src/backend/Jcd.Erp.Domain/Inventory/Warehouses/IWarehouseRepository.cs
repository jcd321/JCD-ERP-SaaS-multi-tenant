namespace Jcd.Erp.Domain.Inventory.Warehouses;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Warehouse> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<bool> HasLocationsAsync(Guid warehouseId, CancellationToken cancellationToken = default);

    Task ClearDefaultExceptAsync(Guid? warehouseId, CancellationToken cancellationToken = default);

    Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default);

    void Update(Warehouse warehouse);

    void Delete(Warehouse warehouse);
}
