using Jcd.Erp.Domain.Inventory.Adjustments;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class InventoryAdjustmentRepository : IInventoryAdjustmentRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryAdjustmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<InventoryAdjustment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.InventoryAdjustments
            .Include(a => a.Warehouse)
            .Include(a => a.Lines)
                .ThenInclude(l => l.Product)
                    .ThenInclude(p => p.Unit)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default)
    {
        var prefix = $"ADJ-{DateTime.UtcNow:yyyyMMdd}-";

        var persistedNumbers = await _context.InventoryAdjustments
            .Where(a => a.DocumentNumber.StartsWith(prefix))
            .Select(a => a.DocumentNumber)
            .ToListAsync(cancellationToken);

        var pendingNumbers = _context.ChangeTracker
            .Entries<InventoryAdjustment>()
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

    public async Task<(IReadOnlyList<InventoryAdjustment> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryAdjustments
            .Include(a => a.Warehouse)
            .Include(a => a.Lines)
                .ThenInclude(l => l.Product)
                    .ThenInclude(p => p.Unit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(a =>
                a.DocumentNumber.ToLower().Contains(term) ||
                a.Reason.ToLower().Contains(term) ||
                a.Warehouse.Code.ToLower().Contains(term) ||
                a.Warehouse.Name.ToLower().Contains(term) ||
                (a.Notes != null && a.Notes.ToLower().Contains(term)));
        }

        if (warehouseId.HasValue)
            query = query.Where(a => a.WarehouseId == warehouseId.Value);

        if (fromDate.HasValue)
            query = query.Where(a => a.AdjustmentDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.AdjustmentDate <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.AdjustmentDate)
            .ThenByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(InventoryAdjustment adjustment, CancellationToken cancellationToken = default) =>
        await _context.InventoryAdjustments.AddAsync(adjustment, cancellationToken);
}
