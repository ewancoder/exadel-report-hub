using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [HasPermission(Resource.Invoices, CrudAction.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvoiceCommand command,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateInvoiceCommand command,
        CancellationToken cancellationToken
    )
    {
        command.Id = id;
        var response = await mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteInvoiceCommand(id);
        var response = await mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetInvoiceByIdQuery(id);
        var response = await mediator.Send(query, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet]
    [HasPermission(Resource.Invoices, CrudAction.Read)]
    public async Task<ActionResult<BaseResponse<PaginatedList<Invoice>>>> GetInvoices(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeDeleted = false
    )
    {
        var query = new GetAllInvoicesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IncludeDeleted = includeDeleted,
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

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalInvoices([FromQuery] GetTotalInvoicesQuery query)
    {
        var response = await mediator.Send(query);

        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("overdue-payments/{clientId}")]
    [HasPermission(Resource.Invoices, CrudAction.Read)]
    public Task<BaseResponse<OverduePaymentsResponse>> GetOverduePayments(
        [FromRoute] Guid clientId,
        CancellationToken cancellationToken
    ) => mediator.Send(new GetOverduePaymentsQuery(clientId), cancellationToken);
}
