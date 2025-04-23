using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using Refit;

namespace ExportPro.Export.SDK.Interfaces;

/// <summary>
/// Refit client. Calls StorageService to fetch invoice data for export.
/// </summary>
public interface IStorageServiceApi
{
    [Get("/api/Invoice/{id}")]
    Task<BaseResponse<InvoiceDto>> GetInvoiceByIdAsync(string id, CancellationToken ct);
}