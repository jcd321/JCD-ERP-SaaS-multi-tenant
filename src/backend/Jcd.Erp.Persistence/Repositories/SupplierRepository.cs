using Jcd.Erp.Domain.Partners.Suppliers;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;

    public SupplierRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return _context.Suppliers.FirstOrDefaultAsync(s => s.Code == normalizedCode, cancellationToken);
    }

    public Task<Supplier?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        var normalizedTaxId = taxId.Trim().ToUpperInvariant();
        return _context.Suppliers.FirstOrDefaultAsync(s => s.TaxId == normalizedTaxId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Supplier> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.Code.ToLower().Contains(term) ||
                s.LegalName.ToLower().Contains(term) ||
                (s.TradeName != null && s.TradeName.ToLower().Contains(term)) ||
                (s.TaxId != null && s.TaxId.ToLower().Contains(term)) ||
                (s.Email != null && s.Email.ToLower().Contains(term)) ||
                (s.Phone != null && s.Phone.Contains(term)) ||
                (s.MobilePhone != null && s.MobilePhone.Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(s => s.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.LegalName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default) =>
        await _context.Suppliers.AddAsync(supplier, cancellationToken);

    public void Update(Supplier supplier) => _context.Suppliers.Update(supplier);

    public void Delete(Supplier supplier)
    {
        supplier.IsDeleted = true;
        supplier.DeletedAt = DateTime.UtcNow;
        _context.Suppliers.Update(supplier);
    }
}
