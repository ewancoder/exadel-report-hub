using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public sealed record CreateCurrencyCommand(string Code) : IRequest<BaseResponse<CurrencyResponse>>;

public sealed class CreateCurrencyHandler(ICurrencyRepository repository, IMapper mapper)
    : IRequestHandler<CreateCurrencyCommand, BaseResponse<CurrencyResponse>>
{
    public async Task<BaseResponse<CurrencyResponse>> Handle(
        CreateCurrencyCommand request,
        CancellationToken cancellationToken
    )
    {
        var currency = new Currency { CurrencyCode = request.Code };
        await repository.AddOneAsync(currency, cancellationToken);
        var curResponse = mapper.Map<CurrencyResponse>(currency);
        return new SuccessResponse<CurrencyResponse>(curResponse, "Currency Successfully created");
    }
}
