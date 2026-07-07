namespace Jcd.Erp.Domain.Partners.Customers;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<Customer?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    void Update(Customer customer);

    void Delete(Customer customer);
}
