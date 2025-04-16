using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.Country;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.Country;

public class GetAllCountriesQueryHandler(ICountryRepository repository) : IQueryHandler<GetAllCountriesQuery, List<Models.Models.Country>>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<List<Models.Models.Country>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync(cancellationToken);
        return new BaseResponse<List<Models.Models.Country>> { Data = result };
    }
}
