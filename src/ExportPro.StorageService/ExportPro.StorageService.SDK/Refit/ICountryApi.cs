using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface ICountryApi
{
    [Post("/api/Country")]
    Task<BaseResponse<CountryDto>> Create(
        [Body] CreateCountryDto country,
        CancellationToken cancellationToken = default
    );

    [Put("/api/Country/{id}")]
    Task<BaseResponse<bool>> Update(
        Guid id,
        [Body] UpdateCountry country,
        CancellationToken cancellationToken = default
    );

    [Delete("/api/Country/{id}")]
    Task<BaseResponse<bool>> Delete(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Country/{id}")]
    Task<BaseResponse<CountryDto>> GetById(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Country")]
    Task<BaseResponse<PaginatedListDto<CountryDto>>> GetAll(
        [Query] int pageNumber = 1,
        [Query] int pageSize = 10,
        CancellationToken cancellationToken = default
    );
}
