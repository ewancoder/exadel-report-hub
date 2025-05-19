using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.SDK.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ExportPro.Export.Pdf.Services;

public sealed class PdfGenerator : IPdfGenerator
{
    public byte[] GeneratePdfDocument(PdfInvoiceExportDto invoice)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigureLayout(page);
                GenerateHeader(invoice, page);
                GenerateTableContent(invoice, page);
                GenerateFooter(page);
            });
        });

        return doc.GeneratePdf();
    }

    private static void GenerateFooter(PageDescriptor page)
    {
        page.Footer().AlignCenter().Text($"Generated {DateTime.UtcNow:u}").Italic();
    }

    private static void GenerateTableContent(PdfInvoiceExportDto invoice, PageDescriptor page)
    {
        page.Content()
            .PaddingVertical(10)
            .Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    for (var i = 0; i < 9; i++)
                        c.RelativeColumn();
                });

                static IContainer Cell(IContainer c)
                {
                    return c.Border(1).BorderColor("#DDD").Padding(4).AlignMiddle().AlignLeft();
                }

                string[] headers =
                [
                    "Customer",
                    "Issue Date",
                    "Due Date",
                    "Item List",
                    "Currency",
                    "Payment Status",
                    "Client",
                    "Bank Acc. #",
                    "Amount",
                ];

                foreach (var h in headers)
                    table.Cell().Element(Cell).Text(h).Bold();

                var itemList = string.Join(
                    "\n",
                    invoice.Items.Select(i => $"{i.Name} — {i.Price:N2} {i.CurrencyCode}")
                );

                string[] row =
                [
                    invoice.CustomerName ?? "-",
                    invoice.IssueDate.ToString("yyyy‑MM‑dd"),
                    invoice.DueDate.ToString("yyyy‑MM‑dd"),
                    itemList,
                    invoice.CurrencyCode ?? "—",
                    invoice.PaymentStatus ?? "—",
                    invoice.ClientName ?? "—",
                    invoice.BankAccountNumber ?? "—",
                    $"{invoice.Amount:N2} {invoice.CurrencyCode}",
                ];

                foreach (var cell in row)
                    table.Cell().Element(Cell).Text(cell).FontSize(8);
            });
    }

    private static void GenerateHeader(PdfInvoiceExportDto invoice, PageDescriptor page)
    {
        page.Header().AlignCenter().Text($"Invoice {invoice.InvoiceNumber}").FontSize(20).Bold();
    }

    private static void ConfigureLayout(PageDescriptor page)
    {
        page.Margin(25);
        page.Size(PageSizes.A4.Landscape());
    }
}
