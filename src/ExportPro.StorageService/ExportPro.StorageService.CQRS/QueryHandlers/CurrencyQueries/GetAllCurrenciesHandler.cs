using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public sealed record GetAllCurrenciesQuery(int Top = 10, int Skip = 0, OrderBy OrderBy = OrderBy.Ascending)
    : IRequest<BaseResponse<List<CurrencyResponse>>>;

public sealed class GetAllCurrenciesHandler(ICurrencyRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCurrenciesQuery, BaseResponse<List<CurrencyResponse>>>
{
    public async Task<BaseResponse<List<CurrencyResponse>>> Handle(
        GetAllCurrenciesQuery request,
        CancellationToken cancellationToken
    )
    {
        var currencies = await repository.GetAllAsync(cancellationToken);
        var currency = currencies.Select(x => mapper.Map<CurrencyResponse>(x)).ToList();
        return new SuccessResponse<List<CurrencyResponse>>(currency, "Currencies retrieved successfully");
    }
}
