using Jcd.Erp.Domain.Inventory.Movements;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryMovementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.InventoryMovements
            .Include(m => m.Product)
                .ThenInclude(p => p.Unit)
            .Include(m => m.Warehouse)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<string> GetNextDocumentNumberAsync(CancellationToken cancellationToken = default)
    {
        var prefix = $"MOV-{DateTime.UtcNow:yyyyMMdd}-";

        var persistedNumbers = await _context.InventoryMovements
            .Where(m => m.DocumentNumber.StartsWith(prefix))
            .Select(m => m.DocumentNumber)
            .ToListAsync(cancellationToken);

        var pendingNumbers = _context.ChangeTracker
            .Entries<InventoryMovement>()
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

    public async Task<(IReadOnlyList<InventoryMovement> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        Guid? warehouseId,
        Guid? productId,
        string? movementType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryMovements
            .Include(m => m.Product)
                .ThenInclude(p => p.Unit)
            .Include(m => m.Warehouse)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(m =>
                m.DocumentNumber.ToLower().Contains(term) ||
                m.Product.Sku.ToLower().Contains(term) ||
                m.Product.Name.ToLower().Contains(term) ||
                m.Warehouse.Code.ToLower().Contains(term) ||
                m.Warehouse.Name.ToLower().Contains(term) ||
                (m.Reference != null && m.Reference.ToLower().Contains(term)));
        }

        if (warehouseId.HasValue)
            query = query.Where(m => m.WarehouseId == warehouseId.Value);

        if (productId.HasValue)
            query = query.Where(m => m.ProductId == productId.Value);

        if (!string.IsNullOrWhiteSpace(movementType) && InventoryMovementTypes.IsValid(movementType))
        {
            var normalizedType = InventoryMovementTypes.Normalize(movementType);
            query = query.Where(m => m.MovementType == normalizedType);
        }

        if (fromDate.HasValue)
            query = query.Where(m => m.MovementDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(m => m.MovementDate <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.MovementDate)
            .ThenByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<InventoryMovement> Items, int TotalCount)> GetKardexPagedAsync(
        Guid productId,
        int page,
        int pageSize,
        Guid? warehouseId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryMovements
            .Include(m => m.Product)
                .ThenInclude(p => p.Unit)
            .Include(m => m.Warehouse)
            .Where(m => m.ProductId == productId);

        if (warehouseId.HasValue)
            query = query.Where(m => m.WarehouseId == warehouseId.Value);

        if (fromDate.HasValue)
            query = query.Where(m => m.MovementDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(m => m.MovementDate <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.MovementDate)
            .ThenBy(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default) =>
        await _context.InventoryMovements.AddAsync(movement, cancellationToken);
}
