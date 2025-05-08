using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public sealed record UpdateCurrencyCommand(Guid CurrencyId, CurrencyDto Currency)
    : IRequest<BaseResponse<CurrencyResponse>>;

public sealed class UpdateCurrencyHandler(
    IHttpContextAccessor httpContext,
    ICurrencyRepository repository,
    IMapper mapper
) : IRequestHandler<UpdateCurrencyCommand, BaseResponse<CurrencyResponse>>
{
    public async Task<BaseResponse<CurrencyResponse>> Handle(
        UpdateCurrencyCommand request,
        CancellationToken cancellationToken
    )
    {
        var currency = await repository.GetOneAsync(
            x => x.Id == request.CurrencyId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (currency == null)
            return new NotFoundResponse<CurrencyResponse>("Currency not Found");
        currency.CurrencyCode = request.Currency.CurrencyCode;
        currency.UpdatedAt = DateTime.UtcNow;
        currency.UpdatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)!.Value;
        await repository.UpdateOneAsync(currency, cancellationToken);
        var currencyResponse = mapper.Map<CurrencyResponse>(currency);
        return new SuccessResponse<CurrencyResponse>(currencyResponse);
    }
}
