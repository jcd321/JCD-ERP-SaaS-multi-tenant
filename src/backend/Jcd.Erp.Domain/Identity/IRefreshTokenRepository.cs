using Jcd.Erp.Domain.Identity;

namespace Jcd.Erp.Domain.Identity;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    void Update(RefreshToken refreshToken);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
