using Jcd.Erp.Domain.Inventory.Transfers;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class InventoryTransferRepository : IInventoryTransferRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryTransferRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<InventoryTransfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.InventoryTransfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Lines)
                .ThenInclude(l => l.Product)
                    .ThenInclude(p => p.Unit)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default)
    {
        var prefix = $"TRF-{DateTime.UtcNow:yyyyMMdd}-";

        var persistedNumbers = await _context.InventoryTransfers
            .Where(t => t.DocumentNumber.StartsWith(prefix))
            .Select(t => t.DocumentNumber)
            .ToListAsync(cancellationToken);

        var pendingNumbers = _context.ChangeTracker
            .Entries<InventoryTransfer>()
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

    public async Task<(IReadOnlyList<InventoryTransfer> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? sourceWarehouseId,
        Guid? destinationWarehouseId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryTransfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Lines)
                .ThenInclude(l => l.Product)
                    .ThenInclude(p => p.Unit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(t =>
                t.DocumentNumber.ToLower().Contains(term) ||
                t.SourceWarehouse.Code.ToLower().Contains(term) ||
                t.SourceWarehouse.Name.ToLower().Contains(term) ||
                t.DestinationWarehouse.Code.ToLower().Contains(term) ||
                t.DestinationWarehouse.Name.ToLower().Contains(term) ||
                (t.Notes != null && t.Notes.ToLower().Contains(term)));
        }

        if (sourceWarehouseId.HasValue)
            query = query.Where(t => t.SourceWarehouseId == sourceWarehouseId.Value);

        if (destinationWarehouseId.HasValue)
            query = query.Where(t => t.DestinationWarehouseId == destinationWarehouseId.Value);

        if (fromDate.HasValue)
            query = query.Where(t => t.TransferDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.TransferDate <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.TransferDate)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(InventoryTransfer transfer, CancellationToken cancellationToken = default) =>
        await _context.InventoryTransfers.AddAsync(transfer, cancellationToken);
}
