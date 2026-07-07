using Jcd.Erp.Application.Common.Interfaces;
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
        TenantProvider tenantProvider)
    {
        if (tenantService.HasTenant)
            tenantProvider.ApplyFrom(tenantService);

        await _next(context);
    }
}
