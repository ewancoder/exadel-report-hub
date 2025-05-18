using ExportPro.Common.Shared.Library;
using ExportPro.Export.CQRS.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExportPro.Export.ServiceHost.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class CustomerImportController(IMediator mediator) : ControllerBase
{
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(SuccessResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse<int>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SuccessResponse<int>>> Bulk(
        [Required] IFormFile file,
        CancellationToken ct)
    {
        var resp = await mediator.Send(new ImportCustomersCommand(file), ct);
        return StatusCode((int)resp.ApiState, resp);
    }
}