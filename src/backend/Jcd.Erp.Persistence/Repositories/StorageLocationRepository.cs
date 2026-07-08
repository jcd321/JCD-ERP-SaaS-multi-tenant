using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class StorageLocationRepository : IStorageLocationRepository
{
    private readonly ApplicationDbContext _context;

    public StorageLocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<StorageLocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.StorageLocations
            .Include(l => l.Parent)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public Task<StorageLocation?> GetByCodeAsync(
        Guid warehouseId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return _context.StorageLocations.FirstOrDefaultAsync(
            l => l.WarehouseId == warehouseId && l.Code == normalizedCode,
            cancellationToken);
    }

    public Task<bool> HasChildrenAsync(Guid locationId, CancellationToken cancellationToken = default) =>
        _context.StorageLocations.AnyAsync(l => l.ParentId == locationId, cancellationToken);

    public async Task<(IReadOnlyList<StorageLocation> Items, int TotalCount)> GetPagedAsync(
        Guid warehouseId,
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StorageLocations
            .Include(l => l.Parent)
            .Where(l => l.WarehouseId == warehouseId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(l =>
                l.Code.ToLower().Contains(term) ||
                l.Name.ToLower().Contains(term) ||
                (l.Description != null && l.Description.ToLower().Contains(term)) ||
                (l.Parent != null && l.Parent.Name.ToLower().Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(l => l.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(l => l.Parent != null ? l.Parent.Name : string.Empty)
            .ThenBy(l => l.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<StorageLocation>> GetParentOptionsAsync(
        Guid warehouseId,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StorageLocations
            .Where(l => l.WarehouseId == warehouseId && l.IsActive);

        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);

        return await query
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StorageLocation location, CancellationToken cancellationToken = default) =>
        await _context.StorageLocations.AddAsync(location, cancellationToken);

    public void Update(StorageLocation location) => _context.StorageLocations.Update(location);

    public void Delete(StorageLocation location)
    {
        location.IsDeleted = true;
        location.DeletedAt = DateTime.UtcNow;
        _context.StorageLocations.Update(location);
    }
}
