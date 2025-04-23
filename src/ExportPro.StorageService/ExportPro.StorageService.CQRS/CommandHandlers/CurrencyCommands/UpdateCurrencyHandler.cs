using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MediatR;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public class UpdateCurrencyCommand : IRequest<BaseResponse<Currency>>
{
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
}
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
                Messages = ["Currency not found."]
            };
        }

        currency.CurrencyCode = request?.Code;
        currency.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOneAsync(currency, cancellationToken);
        return new BaseResponse<Currency> { Data = currency };
    }
}