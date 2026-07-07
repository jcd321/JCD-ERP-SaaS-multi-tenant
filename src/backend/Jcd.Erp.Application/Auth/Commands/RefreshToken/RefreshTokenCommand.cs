using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
