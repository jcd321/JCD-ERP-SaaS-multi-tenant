namespace Jcd.Erp.Domain.Partners.Suppliers;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<Supplier?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Supplier> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task AddAsync(Supplier Supplier, CancellationToken cancellationToken = default);

    void Update(Supplier Supplier);

    void Delete(Supplier Supplier);
}
