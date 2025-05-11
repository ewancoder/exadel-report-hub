using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [HasPermission(Resource.Invoices, CrudAction.Create)]
    public Task<BaseResponse<InvoiceResponse>> Create(
        [FromBody] CreateInvoiceDto invoice,
        CancellationToken cancellationToken
    ) => mediator.Send(new CreateInvoiceCommand(invoice), cancellationToken);

    [HttpPut("{id}")]
    public Task<BaseResponse<InvoiceResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] CreateInvoiceDto invoice,
        CancellationToken cancellationToken
    ) => mediator.Send(new UpdateInvoiceCommand(id, invoice), cancellationToken);

    [HttpDelete("{id}")]
    public Task<BaseResponse<bool>> Delete([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new DeleteInvoiceCommand(id), cancellationToken);

    [HttpGet("{id}")]
    public Task<BaseResponse<InvoiceDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);

    [HttpGet]
    [HasPermission(Resource.Invoices, CrudAction.Read)]
    public Task<BaseResponse<PaginatedListDto<InvoiceDto>>> GetInvoices(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    ) => mediator.Send(new GetAllInvoicesQuery(pageNumber, pageSize), cancellationToken);
}
