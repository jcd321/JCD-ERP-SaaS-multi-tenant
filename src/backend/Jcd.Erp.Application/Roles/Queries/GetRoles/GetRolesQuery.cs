using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Roles.Queries.GetRoles;

public record GetRolesQuery : IRequest<Result<IReadOnlyList<RoleDto>>>;

public record RoleDto(Guid Id, string Name, string? Description, bool IsSystem, IReadOnlyList<string> Permissions);
