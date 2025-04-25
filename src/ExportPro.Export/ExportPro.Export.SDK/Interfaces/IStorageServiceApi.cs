using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.Export.SDK.Interfaces;

/// <summary>
/// Refit client. Calls StorageService to fetch invoice data for export.
/// </summary>
public interface IStorageServiceApi
{
    [Get("/api/Invoice/{id}")]
    Task<BaseResponse<InvoiceDto>> GetInvoiceByIdAsync(string id, CancellationToken cancellationToken);

    [Get("/api/Currency/{id}")]
    Task<BaseResponse<CurrencyResponse>> GetCurrencyByIdAsync(string id, CancellationToken cancellationToken);

    [Get("/api/client/{id}")]
    Task<BaseResponse<ValidationModel<ClientResponse>>> GetClientByIdAsync(string id, CancellationToken cancellationToken);

    [Get("/api/Customer/{id}")]
    Task<BaseResponse<CustomerDto>> GetCustomerByIdAsync(string id, CancellationToken cancellationToken);
}