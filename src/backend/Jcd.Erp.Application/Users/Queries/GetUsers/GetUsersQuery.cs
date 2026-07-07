using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<Result<IReadOnlyList<UserDto>>>;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    bool IsActive,
    IReadOnlyList<string> Roles);
