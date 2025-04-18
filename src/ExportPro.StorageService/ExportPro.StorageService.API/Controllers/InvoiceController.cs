using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Commands.InvoiceCommands;
using ExportPro.StorageService.CQRS.Queries.InvoiceQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoiceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }
   
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
            return BadRequest("Invalid invoice ID format.");

        command.Id = id;

        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid invoice ID.");

        var command = new DeleteInvoiceCommand { Id = objectId };
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var query = new GetInvoiceByIdQuery { Id = id };
        var response = await _mediator.Send(query, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedList<Invoice>>>> GetInvoices(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeDeleted = false)
    {
        var query = new GetAllInvoicesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IncludeDeleted = includeDeleted
        };

        var response = await _mediator.Send(query, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }
}