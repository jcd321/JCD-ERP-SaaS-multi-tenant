namespace Jcd.Erp.Application.Common.Interfaces;

public record TokenResult(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, Guid tenantId, string email, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    string HashToken(string token);
    (Guid UserId, Guid TenantId, string Email)? ValidateAccessToken(string token);
}
