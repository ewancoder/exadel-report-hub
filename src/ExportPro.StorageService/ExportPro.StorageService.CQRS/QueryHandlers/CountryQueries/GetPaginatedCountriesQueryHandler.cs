using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Profiles.CountryMaps;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using System.Net;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CountryQueries;

public record GetPaginatedCountriesQuery(int PageNumber = 1, int PageSize = 10, bool IncludeDeleted = false)
    : IQuery<PaginatedListDto<CountryDto>>;
public class GetPaginatedCountriesQueryHandler(ICountryRepository repository) : IQueryHandler<GetPaginatedCountriesQuery, PaginatedListDto<CountryDto>>
{
    public async Task<BaseResponse<PaginatedListDto<CountryDto>>> Handle(GetPaginatedCountriesQuery request, CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var result = await repository.GetAllPaginatedAsync(parameters, request.IncludeDeleted, cancellationToken);

        var dtoList = result.Items.Select(CountryMapper.ToDto).ToList();

        return new BaseResponse<PaginatedListDto<CountryDto>>
        {
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Data = new PaginatedListDto<CountryDto>(
                dtoList,
                result.TotalCount,
                result.PageNumber,
                result.TotalPages
            )
        };
    }
}