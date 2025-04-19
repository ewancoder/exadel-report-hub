using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Commands.CurrencyCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MediatR;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CurrencyHandlers;

public class UpdateCurrencyHandler(ICurrencyRepository repository) : IRequestHandler<UpdateCurrencyCommand, BaseResponse<Currency>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<Currency>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (currency == null)
        {
            return new BaseResponse<Currency>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Currency not found." }
            };
        }

        currency.CurrencyCode = request.Code;
        currency.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOneAsync(currency,cancellationToken);
        return new BaseResponse<Currency> { Data = currency };
    }
}