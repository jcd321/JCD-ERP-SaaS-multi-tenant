using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Roles.Queries.GetRoles;

public class GetRolesHandler : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var dtos = roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Description,
            r.IsSystem,
            r.RolePermissions.Select(rp => rp.Permission?.Code ?? string.Empty)
                .Where(c => c != string.Empty)
                .ToList())).ToList();

        return Result.Success<IReadOnlyList<RoleDto>>(dtos);
    }
}
