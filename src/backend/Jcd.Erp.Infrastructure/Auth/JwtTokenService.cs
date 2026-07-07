using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Jcd.Erp.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Jcd.Erp.Infrastructure.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly TokenValidationParameters _validationParameters;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        _validationParameters = BuildValidationParameters();
    }

    public string GenerateAccessToken(
        Guid userId,
        Guid tenantId,
        string email,
        IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtClaimTypes.TenantId, tenantId.ToString())
        };

        claims.AddRange(permissions.Select(p => new Claim(JwtClaimTypes.Permission, p)));

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public (Guid UserId, Guid TenantId, string Email)? ValidateAccessToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(token, _validationParameters, out _);
            var userId = GetGuidClaim(principal, JwtRegisteredClaimNames.Sub);
            var tenantId = GetGuidClaim(principal, JwtClaimTypes.TenantId);
            var email = principal.FindFirstValue(JwtRegisteredClaimNames.Email)
                ?? principal.FindFirstValue(ClaimTypes.Email);

            if (userId is null || tenantId is null || string.IsNullOrWhiteSpace(email))
                return null;

            return (userId.Value, tenantId.Value, email);
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }

    private TokenValidationParameters BuildValidationParameters() =>
        new()
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

    private static Guid? GetGuidClaim(ClaimsPrincipal principal, string claimType)
    {
        var value = principal.FindFirstValue(claimType);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
