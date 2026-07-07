namespace Jcd.Erp.Domain.Identity;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
    void Update(PasswordResetToken token);
    Task InvalidateAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
