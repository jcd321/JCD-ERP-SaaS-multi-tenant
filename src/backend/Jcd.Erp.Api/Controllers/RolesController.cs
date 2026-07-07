using Jcd.Erp.Api.Requests;
using Jcd.Erp.Application.Roles.Commands.CreateRole;
using Jcd.Erp.Application.Roles.Commands.DeleteRole;
using Jcd.Erp.Application.Roles.Commands.UpdateRole;
using Jcd.Erp.Application.Roles.Queries.GetPermissions;
using Jcd.Erp.Application.Roles.Queries.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly ISender _mediator;

    public RolesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "roles.view")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetRolesQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("permissions")]
    [Authorize(Policy = "roles.view")]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _mediator.Send(new GetPermissionsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "roles.create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "roles.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var command = new UpdateRoleCommand(id, request.Name, request.Description, request.PermissionCodes);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "roles.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
