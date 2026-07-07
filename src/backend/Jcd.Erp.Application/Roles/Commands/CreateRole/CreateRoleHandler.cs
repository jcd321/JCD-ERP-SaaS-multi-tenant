using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Roles.Commands.CreateRole;

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;
        var name = request.Name.Trim();

        if (await _roleRepository.GetByNameAsync(name, cancellationToken) is not null)
            return Result.Failure<Guid>("Role.NameAlreadyExists");

        var roleResult = Role.Create(tenantId, name, request.Description);
        if (roleResult.IsFailure)
            return Result.Failure<Guid>(roleResult.Error);

        var role = roleResult.Value;
        var assignResult = await RolePermissionAssigner.AssignAsync(
            role,
            request.PermissionCodes,
            _permissionRepository,
            cancellationToken);

        if (assignResult.IsFailure)
            return Result.Failure<Guid>(assignResult.Error);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(role.Id);
    }
}
