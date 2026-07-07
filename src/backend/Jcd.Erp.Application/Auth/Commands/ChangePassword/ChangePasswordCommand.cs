using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Result>;
