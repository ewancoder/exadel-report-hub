using ExportPro.Common.Shared.Attributes;
using ExportPro.StorageService.CQRS.Commands.InvoiceCommands;
using ExportPro.StorageService.CQRS.Queries.invoice;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
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
    [HasPermission(Common.Shared.Enums.Resource.Invoices, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);    
        var res = new InvoiceResponse()
        {
            Id = response.Data.Id.ToString(),
            InvoiceNumber = response.Data.InvoiceNumber,
            DueDate = response.Data.DueDate,
            Amount = response.Data.Amount,
            Currency = response.Data.Currency,
            PaymentStatus = response.Data.PaymentStatus,
            BankAccountNumber = response.Data.BankAccountNumber,
            ClientId = response.Data.ClientId,
            ItemIds = response.Data.ItemIds,
        };
        return StatusCode((int)response.ApiState, res);
    }

    [HttpPut("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Invoices, Common.Shared.Enums.CrudAction.Update)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid invoice ID.");

        command.Id = objectId;
        var response = await _mediator.Send(command, cancellationToken);
        var res = new InvoiceResponse()
        {
            Id = response.Data.Id.ToString(),
            InvoiceNumber = response.Data.InvoiceNumber,
            DueDate = response.Data.DueDate,
            Amount = response.Data.Amount,
            Currency = response.Data.Currency,
            PaymentStatus = response.Data.PaymentStatus,
            BankAccountNumber = response.Data.BankAccountNumber,
            ClientId = response.Data.ClientId,
            ItemIds = response.Data.ItemIds,
        };
        return StatusCode((int)response.ApiState, res);
    }

    [HttpDelete("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Invoices, Common.Shared.Enums.CrudAction.Delete)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid invoice ID.");

        var command = new DeleteInvoiceCommand { Id = objectId };
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Invoices, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid invoice ID.");
        var query = new GetInvoiceByIdQuery { Id = objectId };
        var response = await _mediator.Send(query, cancellationToken);
        var res = new InvoiceResponse()
        {
            Id = response.Data.Id.ToString(),
            InvoiceNumber = response.Data.InvoiceNumber,
            DueDate = response.Data.DueDate,
            Amount = response.Data.Amount,
            Currency = response.Data.Currency,
            PaymentStatus = response.Data.PaymentStatus,
            BankAccountNumber = response.Data.BankAccountNumber,
            ClientId = response.Data.ClientId,
            ItemIds = response.Data.ItemIds,
        };
        return StatusCode((int)response.ApiState,res);
    }

    [HttpGet]
    [HasPermission(Common.Shared.Enums.Resource.Invoices, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllInvoicesQuery(), cancellationToken);
       var res =  response.Data.Select(x => new InvoiceResponse()
        {
            Id = x.Id.ToString(),
            InvoiceNumber = x.InvoiceNumber,
            DueDate = x.DueDate,
            Amount = x.Amount,
            Currency = x.Currency,
            PaymentStatus = x.PaymentStatus,
            BankAccountNumber = x.BankAccountNumber,
            ClientId = x.ClientId,
            ItemIds = x.ItemIds,

        }).ToList();
        return StatusCode((int)response.ApiState, res);
    }
}