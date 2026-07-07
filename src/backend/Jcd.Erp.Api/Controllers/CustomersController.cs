using Jcd.Erp.Api.Requests;
using Jcd.Erp.Application.Customers.Commands.CreateCustomer;
using Jcd.Erp.Application.Customers.Commands.DeleteCustomer;
using Jcd.Erp.Application.Customers.Commands.UpdateCustomer;
using Jcd.Erp.Application.Customers.Queries.GetCustomers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ISender _mediator;

    public CustomersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetCustomersQuery(page, pageSize, search, isActive));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "customers.create")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "customers.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var command = new UpdateCustomerCommand(
            id,
            request.Code,
            request.LegalName,
            request.TradeName,
            request.TaxId,
            request.Email,
            request.Phone,
            request.MobilePhone,
            request.AddressLine1,
            request.City,
            request.StateOrProvince,
            request.CountryCode,
            request.Notes,
            request.IsActive);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "customers.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteCustomerCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
