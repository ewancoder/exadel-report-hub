using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public Task<BaseResponse<CurrencyResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetCurrencyByIdQuery(id), cancellationToken);

    [HttpGet]
    public Task<BaseResponse<List<CurrencyResponse>>> GetAll(CancellationToken cancellationToken) =>
        mediator.Send(new GetAllCurrenciesQuery(), cancellationToken);
}
