using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;

public sealed record GetAllCurrenciesQuery(PaginationParameters PaginationParameters)
    : IQuery<PaginatedList<CurrencyResponse>>;

public sealed class GetAllCurrenciesHandler(ICurrencyRepository repository, IMapper mapper)
    : IQueryHandler<GetAllCurrenciesQuery, PaginatedList<CurrencyResponse>>
{
    public async Task<BaseResponse<PaginatedList<CurrencyResponse>>> Handle(
        GetAllCurrenciesQuery request,
        CancellationToken cancellationToken
    )
    {
        var currencies = await repository.GetPaginated(request.PaginationParameters, cancellationToken);
        return new SuccessResponse<PaginatedList<CurrencyResponse>>(currencies, "Currencies retrieved successfully");
    }
}
