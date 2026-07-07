using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _context.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default) =>
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    public void Update(RefreshToken refreshToken) => _context.RefreshTokens.Update(refreshToken);

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke();
    }
}
