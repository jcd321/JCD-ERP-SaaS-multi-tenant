using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Users.Queries.GetUsers;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public GetUsersHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var dtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = user.UserRoles.Select(ur => ur.Role?.Name ?? string.Empty).Where(n => n != string.Empty).ToList();
            dtos.Add(new UserDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.FullName,
                user.IsActive,
                roles));
        }

        return Result.Success<IReadOnlyList<UserDto>>(dtos);
    }
}
