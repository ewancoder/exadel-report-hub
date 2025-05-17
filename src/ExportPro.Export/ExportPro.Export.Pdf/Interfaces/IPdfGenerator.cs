using ExportPro.Export.SDK.DTOs;

namespace ExportPro.Export.Pdf.Interfaces;

public interface IPdfGenerator
{
    byte[] GeneratePdfDocument(PdfInvoiceExportDto invoice);
}