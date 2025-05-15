using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface ICountryController
{
    [Get("/api/Country/name/{countryCode}")]
    Task<BaseResponse<CountryDto>> GetByCode(string countryCode, CancellationToken cancellationToken = default);

    [Get("/api/Country/{id}")]
    Task<BaseResponse<CountryDto>> GetById(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Country")]
    Task<BaseResponse<PaginatedListDto<CountryDto>>> GetAll(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default
    );
}
