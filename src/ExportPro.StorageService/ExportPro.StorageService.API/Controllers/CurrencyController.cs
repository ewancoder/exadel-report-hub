using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(IMediator mediator) : ControllerBase, ICurrencyController
{
    [HttpGet("name/{currencyCode}")]
    public Task<BaseResponse<CurrencyResponse>> GetByCode(
        [FromRoute] string currencyCode,
        CancellationToken cancellationToken
    ) => mediator.Send(new GetCurrencyByCodeQuery(currencyCode), cancellationToken);

    [HttpGet("{id}")]
    public Task<BaseResponse<CurrencyResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetCurrencyByIdQuery(id), cancellationToken);

    [HttpGet]
    public Task<BaseResponse<List<CurrencyResponse>>> GetAll(
        [FromQuery] Filters filters,
        CancellationToken cancellationToken = default
    ) => mediator.Send(new GetAllCurrenciesQuery(filters), cancellationToken);
}
