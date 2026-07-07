using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string Email, string NewPassword) : IRequest<Result>;
