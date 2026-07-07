using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Persistence.Context;

namespace Jcd.Erp.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
