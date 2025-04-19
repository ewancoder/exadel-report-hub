using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CountryQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class GetPaginatedCountriesQueryHandler(ICountryRepository repository, IMapper mapper) : IQueryHandler<GetPaginatedCountriesQuery, PaginatedListDto<CountryDto>>
{
    public async Task<BaseResponse<PaginatedListDto<CountryDto>>> Handle(GetPaginatedCountriesQuery request, CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var result = await repository.GetAllPaginatedAsync(parameters, request.IncludeDeleted, cancellationToken);

        return new BaseResponse<PaginatedListDto<CountryDto>>
        {
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Data = new PaginatedListDto<CountryDto>(
                mapper.Map<List<CountryDto>>(result.Items),
                result.TotalCount,
                result.PageNumber,
                result.TotalPages
            )
        };
    }
}