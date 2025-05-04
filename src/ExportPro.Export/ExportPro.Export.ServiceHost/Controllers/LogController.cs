using ExportPro.Export.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Export.ServiceHost.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogController(IMediator mediator) : ControllerBase
{
    [HttpGet("today-log")]
    public async Task<IActionResult> DownloadTodaysLog()
    {
        var file = await mediator.Send(new DownloadTodaysLogQuery());
        return file != null
            ? File(file, "text/plain", $"log-{DateTime.UtcNow:yyyyMMdd}.txt")
            : NotFound("Log file not found.");
    }
}