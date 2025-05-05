using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using ExportPro.Export.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Options;

namespace ExportPro.Export.ServiceHost.Controllers;

[Route("api/logs")]
[ApiController]
public class LogController(IMediator mediator) : ControllerBase
{
    [HttpGet("today")]
    public async Task<IActionResult> DownloadTodaysLog()
    {
        var file = await mediator.Send(new DownloadTodaysLogQuery());
        return file != null
            ? File(file, "text/plain", $"log-{DateTime.UtcNow:yyyyMMdd}.txt")
            : NotFound("Log file not found.");
    }

    [HttpGet("range")]
    public async Task<IActionResult> DownloadLogByDateRange(
        [FromQuery] [Required] DateOnly startDate,
        [FromQuery] [Required] DateOnly endDate
    )
    {
        var file = await mediator.Send(new DownloadLogByDateRangeQuery(startDate, endDate));
        return file != null
            ? File(file, "text/plain", $"log-{startDate.ToString()}-{endDate.ToString()}.txt")
            : NotFound("Log file not found.");
    }
}
