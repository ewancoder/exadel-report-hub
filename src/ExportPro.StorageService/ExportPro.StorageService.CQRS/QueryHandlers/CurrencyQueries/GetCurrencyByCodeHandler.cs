using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public sealed record GetCurrencyByCodeQuery(string CurrencyCode) : IRequest<BaseResponse<CurrencyResponse>>;

public sealed class GetCurrencyByCodeHandler(ICurrencyRepository repository, IMapper mapper)
    : IRequestHandler<GetCurrencyByCodeQuery, BaseResponse<CurrencyResponse>>
{
    public async Task<BaseResponse<CurrencyResponse>> Handle(
        GetCurrencyByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var currency = await repository.GetOneAsync(
            x => x.CurrencyCode == request.CurrencyCode.ToUpper(),
            cancellationToken
        );
        var currencyResp = mapper.Map<CurrencyResponse>(currency);
        return currency == null
            ? new NotFoundResponse<CurrencyResponse>("Currency not found.")
            : new SuccessResponse<CurrencyResponse>(currencyResp, "Currecy found successfully.");
    }
}
