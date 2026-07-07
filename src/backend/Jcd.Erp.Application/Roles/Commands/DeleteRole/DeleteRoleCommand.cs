using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(Guid Id) : IRequest<Result>;
