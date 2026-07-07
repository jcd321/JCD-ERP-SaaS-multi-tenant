using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(
    string Name,
    string? Description,
    IReadOnlyList<string> PermissionCodes) : IRequest<Result<Guid>>;
