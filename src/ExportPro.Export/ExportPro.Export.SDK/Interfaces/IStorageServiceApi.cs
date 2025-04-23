using ExportPro.Export.SDK.DTOs;
using Refit;

namespace ExportPro.Export.SDK.Interfaces;

/// <summary>
/// Refit client. Calls StorageService to fetch invoice data for export.
/// </summary>
public interface IStorageServiceApi
{
    [Get("/api/Invoice/{id}")]
    Task<PdfInvoiceExportDto> GetInvoiceByIdAsync(string id, CancellationToken cancellationToken);
}