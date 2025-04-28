using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public sealed class GetAllCurrenciesQuery : IRequest<BaseResponse<List<CurrencyResponse>>> { }
public sealed class GetAllCurrenciesHandler(ICurrencyRepository repository, IMapper mapper) : IRequestHandler<GetAllCurrenciesQuery, BaseResponse<List<CurrencyResponse>>>
{
    private readonly ICurrencyRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    public async Task<BaseResponse<List<CurrencyResponse>>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _repository.GetAllAsync(cancellationToken);
        List<CurrencyResponse> currency = currencies.Select(x => _mapper.Map<CurrencyResponse>(x)).ToList();
        return new BaseResponse<List<CurrencyResponse>> { Data = currency };
    }
}