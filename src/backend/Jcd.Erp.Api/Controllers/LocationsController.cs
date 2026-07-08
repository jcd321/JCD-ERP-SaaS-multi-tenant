using Jcd.Erp.Api.Requests;
using Jcd.Erp.Application.Locations.Commands.CreateLocation;
using Jcd.Erp.Application.Locations.Commands.DeleteLocation;
using Jcd.Erp.Application.Locations.Commands.UpdateLocation;
using Jcd.Erp.Application.Locations.Queries.GetLocationParentOptions;
using Jcd.Erp.Application.Locations.Queries.GetLocations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/locations")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ISender _mediator;

    public LocationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "locations.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid warehouseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetLocationsQuery(warehouseId, page, pageSize, search, isActive));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("parent-options")]
    [Authorize(Policy = "locations.view")]
    public async Task<IActionResult> GetParentOptions(
        [FromQuery] Guid warehouseId,
        [FromQuery] Guid? excludeId = null)
    {
        var result = await _mediator.Send(new GetLocationParentOptionsQuery(warehouseId, excludeId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "locations.create")]
    public async Task<IActionResult> Create([FromBody] CreateLocationCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { warehouseId = command.WarehouseId }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "locations.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationRequest request)
    {
        var command = new UpdateLocationCommand(
            id,
            request.Code,
            request.Name,
            request.Description,
            request.ParentId,
            request.LocationType,
            request.IsActive);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "locations.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteLocationCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
