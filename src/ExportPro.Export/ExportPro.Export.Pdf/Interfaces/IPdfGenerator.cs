using ExportPro.Export.SDK.DTOs;

namespace ExportPro.Export.Pdf.Interfaces;

/// <summary>Abstraction so the export logic isn’t tied to QuestPDF.</summary>
public interface IPdfGenerator
{
    byte[] GeneratePdf(PdfInvoiceExportDto invoice);
}
