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
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtClaimTypes.TenantId);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string? TenantSlug =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtClaimTypes.TenantSlug);

    public bool HasTenant => TenantId != Guid.Empty;
}
