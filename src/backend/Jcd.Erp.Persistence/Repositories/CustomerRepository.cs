using Jcd.Erp.Domain.Partners.Customers;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return _context.Customers.FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);
    }

    public Task<Customer?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        var normalizedTaxId = taxId.Trim().ToUpperInvariant();
        return _context.Customers.FirstOrDefaultAsync(c => c.TaxId == normalizedTaxId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Code.ToLower().Contains(term) ||
                c.LegalName.ToLower().Contains(term) ||
                (c.TradeName != null && c.TradeName.ToLower().Contains(term)) ||
                (c.TaxId != null && c.TaxId.ToLower().Contains(term)) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.Phone != null && c.Phone.Contains(term)) ||
                (c.MobilePhone != null && c.MobilePhone.Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.LegalName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default) =>
        await _context.Customers.AddAsync(customer, cancellationToken);

    public void Update(Customer customer) => _context.Customers.Update(customer);

    public void Delete(Customer customer)
    {
        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        _context.Customers.Update(customer);
    }
}
