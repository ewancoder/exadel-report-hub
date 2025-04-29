using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public class GetAllCurrenciesQuery : IRequest<BaseResponse<List<CurrencyResponse>>> { }

public class GetAllCurrenciesHandler(ICurrencyRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCurrenciesQuery, BaseResponse<List<CurrencyResponse>>>
{
    public async Task<BaseResponse<List<CurrencyResponse>>> Handle(
        GetAllCurrenciesQuery request,
        CancellationToken cancellationToken
    )
    {
        var currencies = await repository.GetAllAsync(cancellationToken);
        List<CurrencyResponse> currency = currencies.Select(x => mapper.Map<CurrencyResponse>(x)).ToList();
        return new BaseResponse<List<CurrencyResponse>> { Data = currency };
    }
}
