using ExportPro.StorageService.CQRS.Commands;
using ExportPro.StorageService.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid customer ID format.");

        command.Id = objectId;

        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    /// <summary>
    /// Soft delete a customer
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid customer ID format.");

        var command = new DeleteCustomerCommand { Id = objectId };
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    /// <summary>
    /// Get a customer by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid customer ID format.");

        var query = new GetCustomerByIdQuery { Id = objectId };
        var response = await _mediator.Send(query, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet("getCustomers")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllCustomersQuery(), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }
}