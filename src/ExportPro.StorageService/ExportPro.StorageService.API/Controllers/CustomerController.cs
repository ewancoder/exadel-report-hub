using ExportPro.StorageService.CQRS.Commands.Customer;
using ExportPro.StorageService.CQRS.Queries.Customer;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
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
        var res = new CustomerResponse()
        {
            Id = response.Data.Id.ToString(),
            Name = response.Data.Name,
            Email = response.Data.Email,
            Country = response.Data.CountryId,
            CreatedAt = response.Data.CreatedAt,
            UpdatedAt = response.Data.UpdatedAt,
        };
        return StatusCode((int)response.ApiState, res);
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
        var res = new CustomerResponse()
        {
            Id = response.Data.Id.ToString(),
            Name = response.Data.Name,
            Email = response.Data.Email,
            Country = response.Data.CountryId,
            CreatedAt = response.Data.CreatedAt,
            UpdatedAt = response.Data.UpdatedAt,
        };
        return StatusCode((int)response.ApiState, res);
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet("getCustomers")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllCustomersQuery(), cancellationToken);
        var res =  response.Data.Select(response => new CustomerResponse()
        {
            Id = response.Id.ToString(),
            Name = response.Name,
            Email = response.Email,
            Country = response.CountryId,
            CreatedAt = response.CreatedAt,
            UpdatedAt = response.UpdatedAt,
        }).ToList();
        return StatusCode((int)response.ApiState, res);
    }
}