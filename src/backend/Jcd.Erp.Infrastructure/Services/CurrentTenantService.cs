using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;

namespace Jcd.Erp.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
                return Guid.Empty;

            var value = user.FindFirstValue(JwtClaimTypes.TenantId)
                ?? user.Claims.FirstOrDefault(c =>
                    c.Type.Equals(JwtClaimTypes.TenantId, StringComparison.OrdinalIgnoreCase)
                    || c.Type.EndsWith("/tenant_id", StringComparison.OrdinalIgnoreCase))?.Value;

            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string? TenantSlug =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtClaimTypes.TenantSlug);

    public bool HasTenant => TenantId != Guid.Empty;
}
