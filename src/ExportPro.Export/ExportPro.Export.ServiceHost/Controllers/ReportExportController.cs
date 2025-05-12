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
    [HttpGet]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<FileResult> Statistics(
        [Required] [FromQuery] ReportFormat format,
        [Required] [FromQuery] Guid clientId,
        CancellationToken cancellationToken = default)
    {
        var filters = new ReportFilterDto { ClientId = clientId };
        var file = await mediator.Send(new GenerateReportQuery(format, filters), cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }
}