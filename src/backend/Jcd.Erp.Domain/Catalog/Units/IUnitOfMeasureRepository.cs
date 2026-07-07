namespace Jcd.Erp.Domain.Catalog.Units;

public interface IUnitOfMeasureRepository
{
    Task<UnitOfMeasure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UnitOfMeasure?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<UnitOfMeasure> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task AddAsync(UnitOfMeasure unit, CancellationToken cancellationToken = default);

    void Update(UnitOfMeasure unit);

    void Delete(UnitOfMeasure unit);
}
