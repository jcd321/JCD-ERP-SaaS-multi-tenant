namespace Jcd.Erp.Domain.Catalog.Brands;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Brand?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Brand> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task AddAsync(Brand brand, CancellationToken cancellationToken = default);

    void Update(Brand brand);

    void Delete(Brand brand);
}
