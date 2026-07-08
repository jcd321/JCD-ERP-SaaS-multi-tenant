using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class WarehouseRepository : IWarehouseRepository
{
    private readonly ApplicationDbContext _context;

    public WarehouseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return _context.Warehouses.FirstOrDefaultAsync(w => w.Code == normalizedCode, cancellationToken);
    }

    public async Task<(IReadOnlyList<Warehouse> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Warehouses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(w =>
                w.Code.ToLower().Contains(term) ||
                w.Name.ToLower().Contains(term) ||
                (w.City != null && w.City.ToLower().Contains(term)) ||
                (w.Description != null && w.Description.ToLower().Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(w => w.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(w => w.IsDefault)
            .ThenBy(w => w.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> HasLocationsAsync(Guid warehouseId, CancellationToken cancellationToken = default) =>
        _context.StorageLocations.AnyAsync(l => l.WarehouseId == warehouseId, cancellationToken);

    public async Task ClearDefaultExceptAsync(Guid? warehouseId, CancellationToken cancellationToken = default)
    {
        var defaults = await _context.Warehouses
            .Where(w => w.IsDefault && (!warehouseId.HasValue || w.Id != warehouseId.Value))
            .ToListAsync(cancellationToken);

        foreach (var warehouse in defaults)
        {
            warehouse.SetDefault(false);
            _context.Warehouses.Update(warehouse);
        }
    }

    public async Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) =>
        await _context.Warehouses.AddAsync(warehouse, cancellationToken);

    public void Update(Warehouse warehouse) => _context.Warehouses.Update(warehouse);

    public void Delete(Warehouse warehouse)
    {
        warehouse.IsDeleted = true;
        warehouse.DeletedAt = DateTime.UtcNow;
        _context.Warehouses.Update(warehouse);
    }
}
