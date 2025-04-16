using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.Country;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.Country;

public class GetCountryByIdQueryHandler(ICountryRepository repository) : IQueryHandler<GetCountryByIdQuery, Models.Models.Country>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<Models.Models.Country>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return new BaseResponse<Models.Models.Country> { Data = result };
    }
}
