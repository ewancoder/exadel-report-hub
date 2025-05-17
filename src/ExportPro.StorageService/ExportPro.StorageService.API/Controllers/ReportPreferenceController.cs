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
    public async Task<BaseResponse<ReportPreferenceResponse>> Update(
        [FromRoute] Guid id, [FromBody] UpdateReportPreferenceDTO pref, CancellationToken cancellationToken) 
            => await mediator.Send(new UpdateReportPreferenceCommand(pref with { Id = id }), cancellationToken);

    [HttpDelete("{id}")]
    public async Task<BaseResponse<ReportPreferenceResponse>> Delete(
        [FromRoute] Guid id, CancellationToken cancellationToken)
            => await mediator.Send(new RemoveReportPreferenceCommand(id), cancellationToken);

    [HttpGet("client/{clientId}")]
    public async Task<BaseResponse<List<ReportPreferenceResponse>>> GetByClient(
        [FromRoute] Guid clientId, CancellationToken cancellationToken)
            => await mediator.Send(new GetReportPreferenceByClientQuery(clientId), cancellationToken);
}