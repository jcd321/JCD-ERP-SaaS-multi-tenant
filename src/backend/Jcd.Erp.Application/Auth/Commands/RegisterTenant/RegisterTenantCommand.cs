using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.RegisterTenant;

public record RegisterTenantCommand(
    string CompanyName,
    string? Slug,
    string AdminEmail,
    string AdminPassword,
    string AdminFirstName,
    string AdminLastName) : IRequest<Result<RegisterTenantResponse>>;

public record RegisterTenantResponse(
    Guid TenantId,
    string TenantSlug,
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
