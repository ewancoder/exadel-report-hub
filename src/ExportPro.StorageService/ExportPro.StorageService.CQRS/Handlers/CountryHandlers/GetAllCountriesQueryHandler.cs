using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CountryQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class GetAllCountriesQueryHandler(ICountryRepository repository) : IQueryHandler<GetAllCountriesQuery, List<Country>>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<List<Country>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync(cancellationToken);
        return new BaseResponse<List<Country>> { Data = result };
    }
}
