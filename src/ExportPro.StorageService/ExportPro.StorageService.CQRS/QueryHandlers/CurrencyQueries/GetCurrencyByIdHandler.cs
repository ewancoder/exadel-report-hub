using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Profiles.CurrencyMaps;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public record GetCurrencyByIdQuery(ObjectId Id) : IRequest<BaseResponse<CurrencyDto>>;

public class GetCurrencyByIdHandler(ICurrencyRepository repository) : IRequestHandler<GetCurrencyByIdQuery, BaseResponse<CurrencyDto>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<CurrencyDto>> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
    {
        var currency = await _repository.GetByIdAsync(request.Id, cancellationToken);
        var currencyResp = CurrencyMapper.ToDto(currency);
        return currency == null
            ? new BaseResponse<CurrencyDto>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Currency not found."]
            }
            : new BaseResponse<CurrencyDto> { Data = currencyResp };
    }
}