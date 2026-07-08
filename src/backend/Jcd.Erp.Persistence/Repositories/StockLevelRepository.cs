using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class StockLevelRepository : IStockLevelRepository
{
    private readonly ApplicationDbContext _context;

    public StockLevelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<StockLevel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.StockLevels
            .Include(s => s.Product)
                .ThenInclude(p => p.Unit)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<StockLevel?> GetByProductAndWarehouseAsync(
        Guid productId,
        Guid warehouseId,
        CancellationToken cancellationToken = default) =>
        _context.StockLevels.FirstOrDefaultAsync(
            s => s.ProductId == productId && s.WarehouseId == warehouseId,
            cancellationToken);

    public async Task<(IReadOnlyList<StockLevel> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        Guid? productId,
        bool? belowMinimumOnly,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StockLevels
            .Include(s => s.Product)
                .ThenInclude(p => p.Unit)
            .Include(s => s.Warehouse)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.Product.Sku.ToLower().Contains(term) ||
                s.Product.Name.ToLower().Contains(term) ||
                s.Warehouse.Code.ToLower().Contains(term) ||
                s.Warehouse.Name.ToLower().Contains(term));
        }

        if (warehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == warehouseId.Value);

        if (productId.HasValue)
            query = query.Where(s => s.ProductId == productId.Value);

        if (belowMinimumOnly == true)
            query = query.Where(s => s.MinQuantity != null && s.QuantityOnHand < s.MinQuantity);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Warehouse.Name)
            .ThenBy(s => s.Product.Sku)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(StockLevel stockLevel, CancellationToken cancellationToken = default) =>
        await _context.StockLevels.AddAsync(stockLevel, cancellationToken);

    public void Update(StockLevel stockLevel) => _context.StockLevels.Update(stockLevel);

    public void Delete(StockLevel stockLevel)
    {
        stockLevel.IsDeleted = true;
        stockLevel.DeletedAt = DateTime.UtcNow;
        _context.StockLevels.Update(stockLevel);
    }
}
