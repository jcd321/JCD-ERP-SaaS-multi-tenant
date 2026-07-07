using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;

namespace Jcd.Erp.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public IReadOnlyList<string> Permissions =>
        _httpContextAccessor.HttpContext?.User
            .FindAll(JwtClaimTypes.Permission)
            .Select(c => c.Value)
            .ToList()
        ?? [];

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
