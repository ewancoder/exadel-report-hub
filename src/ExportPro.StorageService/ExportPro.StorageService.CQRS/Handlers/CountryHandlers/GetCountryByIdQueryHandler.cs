using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CountryQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class GetCountryByIdQueryHandler(ICountryRepository repository) : IQueryHandler<GetCountryByIdQuery, Country>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<Country>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return new BaseResponse<Country> { Data = result };
    }
}
