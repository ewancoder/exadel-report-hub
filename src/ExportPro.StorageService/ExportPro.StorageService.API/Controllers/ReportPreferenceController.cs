using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.ReportPreferenceQueries;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportPreferenceController(IMediator mediator) : ControllerBase
{

    [HttpPost]
    public async Task<BaseResponse<ReportPreferenceResponse>> Create(
        [FromBody] CreateReportPreferencesDTO pref, CancellationToken cancellationToken)
             => await mediator.Send(new CreateReportPreferenceCommand(pref), cancellationToken);

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateReportPreferenceDTO pref,
        CancellationToken cancellationToken
    )
    {
        var command = pref with { Id = id };
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new RemoveReportPreferenceCommand(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetByClient(
        [FromRoute] Guid clientId,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetReportPreferenceByClientQuery(clientId), cancellationToken);
        return Ok(response);
    }
}