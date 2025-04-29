using ExportPro.Export.CQRS.Queries;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Export.ServiceHost.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ReportExportController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Export statistics of invoices, items and plans.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Statistics(
        [FromQuery] string format = "csv",
        [FromQuery] string? clientId = null,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<ReportFormat>(format, true, out var fmt))
            return BadRequest("format must be 'csv' or 'xlsx'");

        var filters = new ReportFilterDto { ClientId = clientId };

        var file = await mediator.Send(
            new GenerateReportQuery(fmt, filters), 
            cancellationToken);

        return File(file.Content, file.ContentType, file.FileName);
    }
}
