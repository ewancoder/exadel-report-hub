using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(IMediator mediator) : ControllerBase
{
    [HttpGet("name/{currencyCode}")]
    public Task<BaseResponse<CurrencyResponse>> GetById(
        [FromRoute] string currencyCode,
        CancellationToken cancellationToken
    ) => mediator.Send(new GetCurrencyByCodeQuery(currencyCode), cancellationToken);

    [HttpGet("{id}")]
    public Task<BaseResponse<CurrencyResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetCurrencyByIdQuery(id), cancellationToken);

    [HttpGet]
    public Task<BaseResponse<List<CurrencyResponse>>> GetAll(
        int top = 10,
        int skip = 0,
        OrderBy orderBy = OrderBy.Ascending,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new GetAllCurrenciesQuery(top, skip, orderBy), cancellationToken);
}
