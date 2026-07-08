using Jcd.Erp.Api.Requests;
using Jcd.Erp.Application.Warehouses.Commands.CreateWarehouse;
using Jcd.Erp.Application.Warehouses.Commands.DeleteWarehouse;
using Jcd.Erp.Application.Warehouses.Commands.UpdateWarehouse;
using Jcd.Erp.Application.Warehouses.Queries.GetWarehouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly ISender _mediator;

    public WarehousesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "warehouses.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetWarehousesQuery(page, pageSize, search, isActive));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "warehouses.create")]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "warehouses.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequest request)
    {
        var command = new UpdateWarehouseCommand(
            id,
            request.Code,
            request.Name,
            request.Description,
            request.AddressLine1,
            request.City,
            request.StateOrProvince,
            request.CountryCode,
            request.IsDefault,
            request.IsActive);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "warehouses.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteWarehouseCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
