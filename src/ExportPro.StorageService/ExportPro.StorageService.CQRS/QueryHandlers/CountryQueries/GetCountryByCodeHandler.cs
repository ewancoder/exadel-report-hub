using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CountryQueries;

public sealed record GetCountryByCodeQuery(string CountryCode) : IQuery<CountryDto>;

public sealed class GetAllCurrenciesHandler(ICountryRepository repository, IMapper mapper)
    : IQueryHandler<GetCountryByCodeQuery, CountryDto>
{
    public async Task<BaseResponse<CountryDto>> Handle(
        GetCountryByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var country = await repository.GetOneAsync(x => x.Code == request.CountryCode.ToUpper(), cancellationToken);

        var countryResp = mapper.Map<CountryDto>(country);
        return countryResp == null
            ? new NotFoundResponse<CountryDto>("Currency not found.")
            : new SuccessResponse<CountryDto>(countryResp, "Currecy found successfully.");
    }
}
