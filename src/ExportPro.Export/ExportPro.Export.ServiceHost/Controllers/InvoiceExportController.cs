using ExportPro.Export.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Export.ServiceHost.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class InvoiceExportController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<FileResult> GetPdf(Guid id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GenerateInvoicePdfQuery(id), cancellationToken);
        return File(dto.Content, "application/pdf", dto.FileName);
    }
}
