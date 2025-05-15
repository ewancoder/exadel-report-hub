using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface ICurrencyController
{
    [Get("/api/Currency/name/{currencyCode}")]
    Task<BaseResponse<CurrencyResponse>> GetById(string currencyCode, CancellationToken cancellationToken = default);

    [Get("/api/Currency/{id}")]
    Task<BaseResponse<CurrencyResponse>> GetById(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Currency")]
    Task<BaseResponse<List<CurrencyResponse>>> GetAll(Filters filters, CancellationToken cancellationToken = default);
}
