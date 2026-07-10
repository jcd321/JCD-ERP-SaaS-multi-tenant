using Jcd.Erp.Application.PhysicalCounts.Commands.CancelPhysicalInventoryCount;
using Jcd.Erp.Application.PhysicalCounts.Commands.CompletePhysicalInventoryCount;
using Jcd.Erp.Application.PhysicalCounts.Commands.CreatePhysicalInventoryCount;
using Jcd.Erp.Application.PhysicalCounts.Commands.UpdatePhysicalInventoryCountLines;
using Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalCountLookups;
using Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCountById;
using Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/physical-counts")]
[Authorize]
public class PhysicalCountsController : ControllerBase
{
    private readonly ISender _mediator;

    public PhysicalCountsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "physicalcounts.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(new GetPhysicalInventoryCountsQuery(
            page,
            pageSize,
            search,
            warehouseId,
            status,
            fromDate,
            toDate));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "physicalcounts.view")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPhysicalInventoryCountByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("lookups")]
    [Authorize(Policy = "physicalcounts.view")]
    public async Task<IActionResult> GetLookups()
    {
        var result = await _mediator.Send(new GetPhysicalCountLookupsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "physicalcounts.create")]
    public async Task<IActionResult> Create([FromBody] CreatePhysicalInventoryCountCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}/lines")]
    [Authorize(Policy = "physicalcounts.update")]
    public async Task<IActionResult> UpdateLines(Guid id, [FromBody] IReadOnlyList<PhysicalInventoryCountLineUpdate> lines)
    {
        var result = await _mediator.Send(new UpdatePhysicalInventoryCountLinesCommand(id, lines));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize(Policy = "physicalcounts.complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _mediator.Send(new CompletePhysicalInventoryCountCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = "physicalcounts.update")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _mediator.Send(new CancelPhysicalInventoryCountCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
