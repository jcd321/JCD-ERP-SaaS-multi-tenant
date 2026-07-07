using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string? TenantSlug,
    bool RememberMe) : IRequest<Result<LoginResponse>>;

public record LoginResponse(
    Guid TenantId,
    string TenantSlug,
    Guid UserId,
    string Email,
    string FullName,
    IReadOnlyList<string> Permissions,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
