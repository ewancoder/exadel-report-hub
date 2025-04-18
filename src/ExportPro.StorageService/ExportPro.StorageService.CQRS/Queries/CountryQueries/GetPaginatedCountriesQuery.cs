using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.CQRS.Queries.CountryQueries;

public record GetPaginatedCountriesQuery(int PageNumber = 1, int PageSize = 10, bool IncludeDeleted = false)
    : IQuery<PaginatedListDto<CountryDto>>;