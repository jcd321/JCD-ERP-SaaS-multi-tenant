namespace Jcd.Erp.Domain.Catalog.Categories;

public interface ICategoryRepository
{
    Task<ProductCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProductCategory?> GetByNameAsync(
        string name,
        Guid? parentId,
        CancellationToken cancellationToken = default);

    Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ProductCategory> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductCategory>> GetAllForParentSelectAsync(
        Guid? excludeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(ProductCategory category, CancellationToken cancellationToken = default);

    void Update(ProductCategory category);

    void Delete(ProductCategory category);
}
