using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Queries.CurrencyQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CurrencyHandlers;

public class GetCurrencyByIdHandler(ICurrencyRepository repository, IMapper mapper) : IRequestHandler<GetCurrencyByIdQuery, BaseResponse<CurrencyResponse>>
{
    private readonly ICurrencyRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<CurrencyResponse>> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
    {
        var currency = await _repository.GetByIdAsync(request.Id, cancellationToken);
        var currencyResp = _mapper.Map<CurrencyResponse>(currency);
        return currency == null
            ? new BaseResponse<CurrencyResponse>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Currency not found."]
            }
            : new BaseResponse<CurrencyResponse> { Data = currencyResp};
    }
}