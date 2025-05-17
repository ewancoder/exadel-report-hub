using Refit;

namespace ExportPro.Export.IntegrationTests;

public interface IInvoiceExport
{
    [Get("/api/InvoiceExport/{id}/pdf")]
    Task<HttpResponseMessage> GetInvoiceExportPdf(Guid id);
}
