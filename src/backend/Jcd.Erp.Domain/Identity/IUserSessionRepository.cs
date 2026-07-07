namespace Jcd.Erp.Domain.Identity;

public interface IUserSessionRepository
{
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);

    Task RevokeActiveForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
