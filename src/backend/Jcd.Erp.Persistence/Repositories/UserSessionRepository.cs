using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class UserSessionRepository : IUserSessionRepository
{
    private readonly ApplicationDbContext _context;

    public UserSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default) =>
        await _context.UserSessions.AddAsync(session, cancellationToken);

    public async Task RevokeActiveForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeSessions = await _context.UserSessions
            .Where(session => session.UserId == userId && !session.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var session in activeSessions)
            session.Revoke();
    }
}
