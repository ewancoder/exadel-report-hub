using ExportPro.Common.Shared.Library;
using ExportPro.Export.CQRS.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Export.ServiceHost.Controllers;

[ApiController]
[Route("api/Customer")]
public sealed class CustomerImportController(IMediator mediator) : ControllerBase
{
    // [HttpPost("bulk")]
    // [Consumes("multipart/form-data")]
    // [ProducesResponseType(typeof(SuccessResponse<int>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(BadRequestResponse<int>), StatusCodes.Status400BadRequest)]
    // public async Task<IActionResult> Bulk(
    //     IFormFile file,
    //     CancellationToken ct)
    // {
    //     var resp = await mediator.Send(new ImportCustomersCommand(file), ct);
    //     return StatusCode((int)resp.ApiState, resp);
    // }
    
    [HttpPost("bulk")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BaseResponse<int>), StatusCodes.Status200OK)]
    public Task<BaseResponse<int>> Bulk(
        IFormFile file,
        CancellationToken ct) =>
        mediator.Send(new ImportCustomersCommand(file), ct);
}