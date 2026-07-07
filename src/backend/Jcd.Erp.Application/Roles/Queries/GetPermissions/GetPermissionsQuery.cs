using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Roles.Queries.GetPermissions;

public record GetPermissionsQuery : IRequest<Result<IReadOnlyList<PermissionDto>>>;

public record PermissionDto(Guid Id, string Code, string Module, string Action, string? Description);
