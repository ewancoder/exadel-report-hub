using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface ICurrencyController
{
    [Get("/api/Currency/name/{currencyCode}")]
    Task<BaseResponse<CurrencyResponse>> GetByCode(string currencyCode, CancellationToken cancellationToken = default);

    [Get("/api/Currency/{id}")]
    Task<BaseResponse<CurrencyResponse>> GetById(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Currency")]
    Task<BaseResponse<PaginatedList<CurrencyResponse>>> GetAll(
        [Query] PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );
}
