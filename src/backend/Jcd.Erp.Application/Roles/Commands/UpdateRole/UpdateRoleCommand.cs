using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<string> PermissionCodes) : IRequest<Result>;
