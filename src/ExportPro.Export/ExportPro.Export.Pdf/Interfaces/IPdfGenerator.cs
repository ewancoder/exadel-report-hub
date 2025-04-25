using ExportPro.Export.SDK.DTOs;

namespace ExportPro.Export.Pdf.Interfaces;

public interface IPdfGenerator
{
    byte[] GeneratePdf(PdfInvoiceExportDto invoice);
}
