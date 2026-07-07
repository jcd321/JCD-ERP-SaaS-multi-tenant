using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.Logout;

public record LogoutCommand(string? RefreshToken) : IRequest<Result>;
