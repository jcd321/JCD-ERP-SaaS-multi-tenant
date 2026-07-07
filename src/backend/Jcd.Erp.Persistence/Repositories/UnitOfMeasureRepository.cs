using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly ApplicationDbContext _context;

    public UnitOfMeasureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<UnitOfMeasure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.UnitsOfMeasure.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<UnitOfMeasure?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return _context.UnitsOfMeasure.FirstOrDefaultAsync(u => u.Code == normalizedCode, cancellationToken);
    }

    public async Task<(IReadOnlyList<UnitOfMeasure> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.UnitsOfMeasure.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                u.Code.ToLower().Contains(term) ||
                u.Name.ToLower().Contains(term) ||
                (u.Symbol != null && u.Symbol.ToLower().Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(UnitOfMeasure unit, CancellationToken cancellationToken = default) =>
        await _context.UnitsOfMeasure.AddAsync(unit, cancellationToken);

    public void Update(UnitOfMeasure unit) => _context.UnitsOfMeasure.Update(unit);

    public void Delete(UnitOfMeasure unit)
    {
        unit.IsDeleted = true;
        unit.DeletedAt = DateTime.UtcNow;
        _context.UnitsOfMeasure.Update(unit);
    }
}
