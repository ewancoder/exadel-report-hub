using ExportPro.Export.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Export.ServiceHost.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class InvoiceExportController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(string id, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GeneratePdfInvoiceQuery(id), cancellationToken);
        return File(dto.Content, "application/pdf", dto.FileName);
    }
}
