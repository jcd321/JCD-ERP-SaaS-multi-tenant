using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Persistence.Context;

namespace Jcd.Erp.Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenantService tenantService,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        TenantProvider tenantProvider)
    {
        if (tenantService.HasTenant)
        {
            tenantProvider.ApplyFrom(tenantService);
        }
        else if (currentUserService.IsAuthenticated && currentUserService.UserId is Guid userId)
        {
            var user = await userRepository.GetByIdIgnoringFiltersAsync(userId, context.RequestAborted);
            if (user is not null)
                tenantProvider.SetTenant(user.TenantId);
        }

        await _next(context);
    }
}
