using Jcd.Erp.Application.Transfers.Commands.CreateInventoryTransfer;
using Jcd.Erp.Application.Transfers.Queries.GetInventoryTransfers;
using Jcd.Erp.Application.Transfers.Queries.GetTransferLookups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/transfers")]
[Authorize]
public class TransfersController : ControllerBase
{
    private readonly ISender _mediator;

    public TransfersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "transfers.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? sourceWarehouseId = null,
        [FromQuery] Guid? destinationWarehouseId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(new GetInventoryTransfersQuery(
            page,
            pageSize,
            search,
            sourceWarehouseId,
            destinationWarehouseId,
            fromDate,
            toDate));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("lookups")]
    [Authorize(Policy = "transfers.view")]
    public async Task<IActionResult> GetLookups()
    {
        var result = await _mediator.Send(new GetTransferLookupsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "transfers.create")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryTransferCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }
}
