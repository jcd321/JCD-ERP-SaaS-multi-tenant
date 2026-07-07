using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    IReadOnlyList<Guid> RoleIds) : IRequest<Result<Guid>>;
