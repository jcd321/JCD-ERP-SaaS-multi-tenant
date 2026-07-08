namespace Jcd.Erp.Domain.Inventory.Stock;

public interface IStockLevelRepository
{
    Task<StockLevel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StockLevel?> GetByProductAndWarehouseAsync(
        Guid productId,
        Guid warehouseId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<StockLevel> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        Guid? productId,
        bool? belowMinimumOnly,
        CancellationToken cancellationToken = default);

    Task AddAsync(StockLevel stockLevel, CancellationToken cancellationToken = default);

    void Update(StockLevel stockLevel);

    void Delete(StockLevel stockLevel);
}
