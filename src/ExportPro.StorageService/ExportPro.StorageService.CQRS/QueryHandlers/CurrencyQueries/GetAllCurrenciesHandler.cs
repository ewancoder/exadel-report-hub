using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Profiles.CurrencyMaps;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public class GetAllCurrenciesQuery : IRequest<BaseResponse<List<CurrencyDto>>> { }
public class GetAllCurrenciesHandler(ICurrencyRepository repository) : IRequestHandler<GetAllCurrenciesQuery, BaseResponse<List<CurrencyDto>>>
{
    private readonly ICurrencyRepository _repository = repository;
    public async Task<BaseResponse<List<CurrencyDto>>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _repository.GetAllAsync(cancellationToken);
        List<CurrencyDto> currency = currencies.Select(CurrencyMapper.ToDto).ToList();
        return new BaseResponse<List<CurrencyDto>> { Data = currency };
    }
}