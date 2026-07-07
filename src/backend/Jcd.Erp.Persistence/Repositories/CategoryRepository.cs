using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<ProductCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.ProductCategories
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<ProductCategory?> GetByNameAsync(
        string name,
        Guid? parentId,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        return _context.ProductCategories.FirstOrDefaultAsync(
            c => c.Name == normalizedName && c.ParentId == parentId,
            cancellationToken);
    }

    public Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        _context.ProductCategories.AnyAsync(c => c.ParentId == categoryId, cancellationToken);

    public async Task<(IReadOnlyList<ProductCategory> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductCategories
            .Include(c => c.Parent)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Name.ToLower().Contains(term) ||
                (c.Description != null && c.Description.ToLower().Contains(term)) ||
                (c.Parent != null && c.Parent.Name.ToLower().Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Parent != null ? c.Parent.Name : string.Empty)
            .ThenBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<ProductCategory>> GetAllForParentSelectAsync(
        Guid? excludeId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductCategories
            .Where(c => c.IsActive)
            .AsQueryable();

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ProductCategory category, CancellationToken cancellationToken = default) =>
        await _context.ProductCategories.AddAsync(category, cancellationToken);

    public void Update(ProductCategory category) => _context.ProductCategories.Update(category);

    public void Delete(ProductCategory category)
    {
        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        _context.ProductCategories.Update(category);
    }
}
