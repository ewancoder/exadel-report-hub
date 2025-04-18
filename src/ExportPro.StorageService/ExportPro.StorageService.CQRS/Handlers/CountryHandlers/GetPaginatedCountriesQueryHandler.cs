using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CountryQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class GetPaginatedCountriesQueryHandler(ICountryRepository repository, IMapper mapper)
    : IQueryHandler<GetPaginatedCountriesQuery, PaginatedListDto<CountryDto>>
{
    public async Task<BaseResponse<PaginatedListDto<CountryDto>>> Handle(GetPaginatedCountriesQuery request, CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var result = await repository.GetAllPaginatedAsync(parameters, request.IncludeDeleted, cancellationToken);
        return new BaseResponse<PaginatedListDto<CountryDto>>
        {
            Data = new PaginatedListDto<CountryDto>()
            {
                Items = mapper.Map<List<CountryDto>>(result.Items),
                PageNumber = result.PageNumber,
                PageSize = result.Items.Count,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount
            },
            IsSuccess = true,
            ApiState = HttpStatusCode.OK
        };
    }
}
