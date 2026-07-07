using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;
