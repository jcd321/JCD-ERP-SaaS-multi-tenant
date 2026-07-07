using Jcd.Erp.Domain.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Jcd.Erp.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var (module, action, _) in PermissionCatalog.All)
            {
                var code = $"{module}.{action}";
                options.AddPolicy(code, policy =>
                    policy.RequireClaim("permission", code));
            }
        });

        return services;
    }
}
