using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class PhysicalInventoryCountRepository : IPhysicalInventoryCountRepository
{
    private readonly ApplicationDbContext _context;

    public PhysicalInventoryCountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<PhysicalInventoryCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.PhysicalInventoryCounts
            .Include(c => c.Warehouse)
            .Include(c => c.Lines)
                .ThenInclude(l => l.Product)
                    .ThenInclude(p => p.Unit)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default)
    {
        var prefix = $"PIC-{DateTime.UtcNow:yyyyMMdd}-";

        var persistedNumbers = await _context.PhysicalInventoryCounts
            .Where(c => c.DocumentNumber.StartsWith(prefix))
            .Select(c => c.DocumentNumber)
            .ToListAsync(cancellationToken);

        var pendingNumbers = _context.ChangeTracker
            .Entries<PhysicalInventoryCount>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity.DocumentNumber)
            .Where(documentNumber => documentNumber.StartsWith(prefix));

        var maxSequence = persistedNumbers
            .Concat(pendingNumbers)
            .Select(documentNumber => ParseSequence(documentNumber, prefix))
            .DefaultIfEmpty(0)
            .Max();

        return $"{prefix}{(maxSequence + 1):D3}";
    }

    private static int ParseSequence(string documentNumber, string prefix)
    {
        if (!documentNumber.StartsWith(prefix))
            return 0;

        var suffix = documentNumber[prefix.Length..];
        return int.TryParse(suffix, out var sequence) ? sequence : 0;
    }

    public async Task<(IReadOnlyList<PhysicalInventoryCount> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.PhysicalInventoryCounts
            .Include(c => c.Warehouse)
            .Include(c => c.Lines)
                .ThenInclude(l => l.Product)
                    .ThenInclude(p => p.Unit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.DocumentNumber.ToLower().Contains(term) ||
                c.Warehouse.Code.ToLower().Contains(term) ||
                c.Warehouse.Name.ToLower().Contains(term) ||
                (c.Notes != null && c.Notes.ToLower().Contains(term)));
        }

        if (warehouseId.HasValue)
            query = query.Where(c => c.WarehouseId == warehouseId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Status == status.Trim().ToUpperInvariant());

        if (fromDate.HasValue)
            query = query.Where(c => c.CountDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.CountDate <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CountDate)
            .ThenByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(PhysicalInventoryCount count, CancellationToken cancellationToken = default) =>
        await _context.PhysicalInventoryCounts.AddAsync(count, cancellationToken);

    public void Update(PhysicalInventoryCount count) => _context.PhysicalInventoryCounts.Update(count);
}
