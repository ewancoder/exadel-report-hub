using System.ComponentModel.DataAnnotations;
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
        [Required, FromQuery] ReportFormat format,
        [FromQuery] Guid? clientId = null,
        CancellationToken cancellationToken = default)
    {
        var filters = new ReportFilterDto { ClientId = clientId };
        var file = await mediator.Send(new GenerateReportQuery(format, filters), cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("list")]
    public async Task<IActionResult> StatisticsList(
        [Required, FromQuery] ReportFormat format,
        [Required, FromQuery(Name = "clientIds")]
        List<Guid> clientIds,
        [FromQuery] DateTime? issueDateFrom = null,
        CancellationToken cancellationToken = default)
    {
        if (clientIds.Count == 0)
            return BadRequest("At least one client id must be supplied.");

        var filters = new ReportFilterDto
        {
            ClientIds = clientIds,
            IssueDateFrom = issueDateFrom
        };

        var file = await mediator.Send(new GenerateReportQuery(format, filters), cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }
}