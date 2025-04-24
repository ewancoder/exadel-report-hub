using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Profiles.CurrencyMaps;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public record CreateCurrencyCommand(string Code) : IRequest<BaseResponse<CurrencyDto>>;
public class CreateCurrencyHandler(ICurrencyRepository repository) : IRequestHandler<CreateCurrencyCommand, BaseResponse<CurrencyDto>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<CurrencyDto>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = new Currency
        {
            CurrencyCode = request.Code
        };

        await _repository.AddOneAsync(currency, cancellationToken);
        var curResponse = CurrencyMapper.ToDto(currency);
        return new BaseResponse<CurrencyDto> { Data = curResponse };
    }
}