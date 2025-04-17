using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Queries.CurrencyQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MediatR;

namespace ExportPro.StorageService.CQRS.Handlers.CurrencyHandlers;

public class GetAllCurrenciesHandler(ICurrencyRepository repository) : IRequestHandler<GetAllCurrenciesQuery, BaseResponse<List<Currency>>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<List<Currency>>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _repository.GetAllAsync(cancellationToken);
        return new BaseResponse<List<Currency>>{ Data = currencies};
    }
}