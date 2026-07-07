using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    bool IsActive,
    IReadOnlyList<Guid> RoleIds) : IRequest<Result>;
