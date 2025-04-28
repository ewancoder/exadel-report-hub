using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [HasPermission(Common.Shared.Enums.Resource.Invoices, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }
   
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
            return BadRequest("Invalid invoice ID format.");

        command.Id = id;

        var response = await mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid invoice ID.");

        var command = new DeleteInvoiceCommand(objectId);
        var response = await mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var query = new GetInvoiceByIdQuery(id);
        var response = await mediator.Send(query, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet]
    [HasPermission(Common.Shared.Enums.Resource.Invoices, Common.Shared.Enums.CrudAction.Read)]
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

        var response = await mediator.Send(query, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetTotalRevenue([FromQuery] GetTotalRevenueQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode((int)result.ApiState, result);
    }
}