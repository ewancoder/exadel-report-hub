using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public record UpdateCurrencyCommand(Guid CurrencyId, string CurrencyCode) : IRequest<BaseResponse<CurrencyResponse>>;

public class UpdateCurrencyHandler(ICurrencyRepository repository, IMapper mapper)
    : IRequestHandler<UpdateCurrencyCommand, BaseResponse<CurrencyResponse>>
{
    public async Task<BaseResponse<CurrencyResponse>> Handle(
        UpdateCurrencyCommand request,
        CancellationToken cancellationToken
    )
    {
        var currency = await repository.GetByIdAsync(request.CurrencyId.ToObjectId(), cancellationToken);
        if (currency == null)
        {
            return new BaseResponse<CurrencyResponse>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Currency not found."],
            };
        }
        currency.CurrencyCode = request.CurrencyCode;
        currency.UpdatedAt = DateTime.UtcNow;
        await repository.UpdateOneAsync(currency, cancellationToken);
        var currencyResponse = mapper.Map<CurrencyResponse>(currency);
        return new SuccessResponse<CurrencyResponse>(currencyResponse);
    }
}
