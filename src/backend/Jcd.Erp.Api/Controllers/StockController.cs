using Jcd.Erp.Api.Requests;
using Jcd.Erp.Application.Stock.Commands.CreateStockLevel;
using Jcd.Erp.Application.Stock.Commands.DeleteStockLevel;
using Jcd.Erp.Application.Stock.Commands.UpdateStockLevel;
using Jcd.Erp.Application.Stock.Queries.GetStockLevels;
using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/stock")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly ISender _mediator;

    public StockController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "stock.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? productId = null,
        [FromQuery] bool? belowMinimumOnly = null)
    {
        var result = await _mediator.Send(new GetStockLevelsQuery(
            page,
            pageSize,
            search,
            warehouseId,
            productId,
            belowMinimumOnly));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("lookups")]
    [Authorize(Policy = "stock.view")]
    public async Task<IActionResult> GetLookups()
    {
        var result = await _mediator.Send(new GetStockLookupsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "stock.create")]
    public async Task<IActionResult> Create([FromBody] CreateStockLevelCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "stock.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStockLevelRequest request)
    {
        var command = new UpdateStockLevelCommand(
            id,
            request.QuantityOnHand,
            request.MinQuantity,
            request.MaxQuantity);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "stock.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteStockLevelCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
