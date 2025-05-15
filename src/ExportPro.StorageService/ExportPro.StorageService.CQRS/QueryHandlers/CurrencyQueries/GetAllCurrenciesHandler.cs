using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public sealed record GetAllCurrenciesQuery(Filters FiltersCurrencies) : IQuery<List<CurrencyResponse>>;

public sealed class GetAllCurrenciesHandler(ICurrencyRepository repository, IMapper mapper)
    : IQueryHandler<GetAllCurrenciesQuery, List<CurrencyResponse>>
{
    public async Task<BaseResponse<List<CurrencyResponse>>> Handle(
        GetAllCurrenciesQuery request,
        CancellationToken cancellationToken
    )
    {
        var currencies = await repository.GetPaginated(request.FiltersCurrencies, cancellationToken);
        var currency = currencies.Select(x => mapper.Map<CurrencyResponse>(x)).ToList();
        return new SuccessResponse<List<CurrencyResponse>>(currency, "Currencies retrieved successfully");
    }
}
