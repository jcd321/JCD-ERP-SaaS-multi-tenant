using Jcd.Erp.Application.Adjustments.Commands.CreateInventoryAdjustment;
using Jcd.Erp.Application.Adjustments.Queries.GetAdjustmentLookups;
using Jcd.Erp.Application.Adjustments.Queries.GetInventoryAdjustments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/adjustments")]
[Authorize]
public class AdjustmentsController : ControllerBase
{
    private readonly ISender _mediator;

    public AdjustmentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "adjustments.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(new GetInventoryAdjustmentsQuery(
            page,
            pageSize,
            search,
            warehouseId,
            fromDate,
            toDate));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("lookups")]
    [Authorize(Policy = "adjustments.view")]
    public async Task<IActionResult> GetLookups()
    {
        var result = await _mediator.Send(new GetAdjustmentLookupsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "adjustments.create")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryAdjustmentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }
}
