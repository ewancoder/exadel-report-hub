using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public sealed record UpdateCurrencyCommand(Guid CurrencyId, string CurrencyCode)
    : IRequest<BaseResponse<CurrencyResponse>>;

public sealed class UpdateCurrencyHandler(ICurrencyRepository repository, IMapper mapper)
    : IRequestHandler<UpdateCurrencyCommand, BaseResponse<CurrencyResponse>>
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
        currency.CurrencyCode = request.CurrencyCode;
        currency.UpdatedAt = DateTime.UtcNow;
        await repository.UpdateOneAsync(currency, cancellationToken);
        var currencyResponse = mapper.Map<CurrencyResponse>(currency);
        return new SuccessResponse<CurrencyResponse>(currencyResponse);
    }
}
