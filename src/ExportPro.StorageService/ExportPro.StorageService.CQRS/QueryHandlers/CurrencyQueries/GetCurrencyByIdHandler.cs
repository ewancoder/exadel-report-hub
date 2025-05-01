using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public sealed record GetCurrencyByIdQuery(Guid Id) : IRequest<BaseResponse<CurrencyResponse>>;

public sealed class GetCurrencyByIdHandler(ICurrencyRepository repository, IMapper mapper)
    : IRequestHandler<GetCurrencyByIdQuery, BaseResponse<CurrencyResponse>>
{
    public async Task<BaseResponse<CurrencyResponse>> Handle(
        GetCurrencyByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var currency = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        var currencyResp = mapper.Map<CurrencyResponse>(currency);
        return currency == null
            ? new NotFoundResponse<CurrencyResponse>("Currency not found.")
            : new SuccessResponse<CurrencyResponse>(currencyResp, "Currecy found successfully.");
    }
}
