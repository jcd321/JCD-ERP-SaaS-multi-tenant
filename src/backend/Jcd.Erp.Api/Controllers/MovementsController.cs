using Jcd.Erp.Application.Movements.Commands.CreateInventoryMovement;
using Jcd.Erp.Application.Movements.Queries.GetInventoryMovements;
using Jcd.Erp.Application.Movements.Queries.GetMovementLookups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/movements")]
[Authorize]
public class MovementsController : ControllerBase
{
    private readonly ISender _mediator;

    public MovementsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "movements.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? productId = null,
        [FromQuery] string? movementType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(new GetInventoryMovementsQuery(
            page,
            pageSize,
            search,
            warehouseId,
            productId,
            movementType,
            fromDate,
            toDate));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("lookups")]
    [Authorize(Policy = "movements.view")]
    public async Task<IActionResult> GetLookups()
    {
        var result = await _mediator.Send(new GetMovementLookupsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "movements.create")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryMovementCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }
}
