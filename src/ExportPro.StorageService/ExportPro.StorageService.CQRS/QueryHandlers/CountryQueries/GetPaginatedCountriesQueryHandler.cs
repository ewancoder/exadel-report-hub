using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CountryQueries;

public sealed record GetPaginatedCountriesQuery(PaginationParameters PaginationParameters)
    : IQuery<PaginatedListDto<CountryDto>>;

public sealed class GetPaginatedCountriesQueryHandler(ICountryRepository repository, IMapper mapper)
    : IQueryHandler<GetPaginatedCountriesQuery, PaginatedListDto<CountryDto>>
{
    public async Task<BaseResponse<PaginatedListDto<CountryDto>>> Handle(
        GetPaginatedCountriesQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = await repository.GetAllPaginatedAsync(request.PaginationParameters, cancellationToken);

        return new SuccessResponse<PaginatedListDto<CountryDto>>(
            new PaginatedListDto<CountryDto>(
                mapper.Map<List<CountryDto>>(result.Items),
                result.TotalCount,
                result.PageNumber,
                result.TotalPages
            )
        );
    }
}
