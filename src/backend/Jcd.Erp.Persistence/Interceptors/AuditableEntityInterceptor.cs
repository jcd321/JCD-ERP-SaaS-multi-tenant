using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Jcd.Erp.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeService _dateTime;
    private readonly ICurrentUserService _currentUser;

    public AuditableEntityInterceptor(
        IDateTimeService dateTime,
        ICurrentUserService currentUser)
    {
        _dateTime = dateTime;
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = _dateTime.UtcNow;
        var userId = _currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}
