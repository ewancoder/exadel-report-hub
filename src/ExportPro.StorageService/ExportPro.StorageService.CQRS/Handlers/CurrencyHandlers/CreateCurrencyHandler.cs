using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Commands.CurrencyCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MediatR;

namespace ExportPro.StorageService.CQRS.Handlers.CurrencyHandlers;

public class CreateCurrencyHandler(ICurrencyRepository repository) : IRequestHandler<CreateCurrencyCommand, BaseResponse<Currency>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<Currency>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = new Currency
        {
            Name = request.Name,
            Code = request.Code
        };

        await _repository.AddOneAsync(currency, cancellationToken);
        return new BaseResponse<Currency> { Data = currency};
    }
}