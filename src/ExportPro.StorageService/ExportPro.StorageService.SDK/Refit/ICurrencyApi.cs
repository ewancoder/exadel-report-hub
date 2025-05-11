using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface ICurrencyApi
{
    [Post("/api/Currency")]
    Task<BaseResponse<CurrencyResponse>> Create(
        [Body] CurrencyDto currency,
        CancellationToken cancellationToken = default
    );

    [Put("/api/Currency/{id}")]
    Task<BaseResponse<CurrencyResponse>> Update(
        Guid id,
        [Body] string currencyCode,
        CancellationToken cancellationToken = default
    );

    [Delete("/api/Currency/{id}")]
    Task<BaseResponse<bool>> Delete(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Currency/{id}")]
    Task<BaseResponse<CurrencyResponse>> GetById(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Currency")]
    Task<BaseResponse<List<CurrencyResponse>>> GetAll(CancellationToken cancellationToken = default);
}
