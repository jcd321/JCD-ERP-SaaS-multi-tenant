using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class BrandRepository : IBrandRepository
{
    private readonly ApplicationDbContext _context;

    public BrandRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Brand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Brands.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<Brand?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return _context.Brands.FirstOrDefaultAsync(b => b.Code == normalizedCode, cancellationToken);
    }

    public async Task<(IReadOnlyList<Brand> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Brands.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(b =>
                b.Code.ToLower().Contains(term) ||
                b.Name.ToLower().Contains(term) ||
                (b.Description != null && b.Description.ToLower().Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(b => b.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Brand brand, CancellationToken cancellationToken = default) =>
        await _context.Brands.AddAsync(brand, cancellationToken);

    public void Update(Brand brand) => _context.Brands.Update(brand);

    public void Delete(Brand brand)
    {
        brand.IsDeleted = true;
        brand.DeletedAt = DateTime.UtcNow;
        _context.Brands.Update(brand);
    }
}
