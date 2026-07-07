namespace Jcd.Erp.Application.Common.Interfaces;

public interface ICurrentTenantService
{
    Guid TenantId { get; }
    string? TenantSlug { get; }
    bool HasTenant { get; }
}
