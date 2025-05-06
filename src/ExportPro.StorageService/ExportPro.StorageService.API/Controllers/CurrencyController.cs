using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public Task<BaseResponse<CurrencyResponse>> Create(
        [FromBody] string currencyCode,
        CancellationToken cancellationToken
    ) => mediator.Send(new CreateCurrencyCommand(currencyCode), cancellationToken);

    [HttpPut("{id}")]
    public Task<BaseResponse<CurrencyResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] string currencyCode,
        CancellationToken cancellationToken
    ) => mediator.Send(new UpdateCurrencyCommand(id, currencyCode));

    [HttpDelete("{id}")]
    public Task<BaseResponse<bool>> Delete([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new DeleteCurrencyCommand(id), cancellationToken);

    [HttpGet("{id}")]
    public Task<BaseResponse<CurrencyResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetCurrencyByIdQuery(id), cancellationToken);

    [HttpGet]
    public Task<BaseResponse<List<CurrencyResponse>>> GetAll(CancellationToken cancellationToken) =>
        mediator.Send(new GetAllCurrenciesQuery(), cancellationToken);
}
