using System.ComponentModel.DataAnnotations;
using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.ModelFilters;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InvoiceController(IMediator mediator) : ControllerBase, IInvoiceController
{
    [HttpPost]
    public Task<BaseResponse<InvoiceDto>> Create(
        [FromBody] CreateInvoiceDto invoice,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new CreateInvoiceCommand(invoice), cancellationToken);

    [HttpPut("{id}")]
    public Task<BaseResponse<InvoiceResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] CreateInvoiceDto invoice,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new UpdateInvoiceCommand(id, invoice), cancellationToken);

    [HttpDelete("{id}")]
    public Task<BaseResponse<bool>> Delete([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new DeleteInvoiceCommand(id), cancellationToken);

    [HttpGet("{id}")]
    public Task<BaseResponse<InvoiceDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);

    [HttpGet]
    public Task<BaseResponse<PaginatedListDto<InvoiceDto>>> GetInvoices(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new GetAllInvoicesQuery(pageNumber, pageSize), cancellationToken);

    [HttpGet("getByFilter")]
    public Task<BaseResponse<PaginatedList<InvoiceDto>>> GetInvoicesByFilter(
        [FromQuery] InvoiceFilter filters,
        [FromQuery] Guid clientId,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new GetInvoicesByFilter(filters, clientId), cancellationToken);

    [HttpGet("count")]
    public Task<BaseResponse<long>> GetTotalInvoices(
        [FromQuery] TotalInvoicesDto query,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new GetTotalInvoicesQuery(query), cancellationToken);

    [HttpGet("revenue")]
    public Task<BaseResponse<double>> GetTotalRevenue(
        [FromQuery] TotalRevenueDto query,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new GetTotalRevenueQuery(query), cancellationToken);

    [HttpGet("overdue-payments/{clientId}")]
    public Task<BaseResponse<OverduePaymentsResponse>> GetOverduePayments(
        [FromRoute] Guid clientId,
        [Required] [FromQuery] Guid clientCurrencyId,
        CancellationToken cancellationToken
    ) => mediator.Send(new GetOverduePaymentsQuery(clientId, clientCurrencyId), cancellationToken);
}
