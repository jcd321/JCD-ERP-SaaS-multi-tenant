using Jcd.Erp.Domain.Audit;
using Jcd.Erp.Persistence.Context;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default) =>
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
}
