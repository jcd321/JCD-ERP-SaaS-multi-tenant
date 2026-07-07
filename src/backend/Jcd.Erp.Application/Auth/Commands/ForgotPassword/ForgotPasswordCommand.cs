using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email, string? TenantSlug) : IRequest<Result>;
