using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Queries.CurrencyQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MediatR;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CurrencyHandlers;

public class GetCurrencyByIdHandler(ICurrencyRepository repository) : IRequestHandler<GetCurrencyByIdQuery, BaseResponse<Currency>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<Currency>> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
    {
        var currency = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return currency == null
            ? new BaseResponse<Currency>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Currency not found." }
            }
            : new BaseResponse<Currency> { Data = currency };
    }
}