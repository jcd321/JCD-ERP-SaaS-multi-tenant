using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jcd.Erp.Persistence.Repositories;

public sealed class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordResetTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _context.PasswordResetTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

    public async Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default) =>
        await _context.PasswordResetTokens.AddAsync(token, cancellationToken);

    public void Update(PasswordResetToken token) => _context.PasswordResetTokens.Update(token);

    public async Task InvalidateAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeTokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.MarkUsed();
    }
}
