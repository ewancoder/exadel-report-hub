using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.Pdf.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ExportPro.Export.Pdf.Services;

public sealed class PdfGenerator : IPdfGenerator
{
    public byte[] GeneratePdf(PdfInvoiceExportDto invoice)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.A4);

                // ---- Header ----
                page.Header()
                    .AlignCenter()
                    .Text($"Invoice {invoice.InvoiceNumber}")
                    .FontSize(20)
                    .Bold();

                // ---- Table ----
                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        for (int i = 0; i < 10; i++) c.RelativeColumn();
                    });

                    static IContainer Cell(IContainer c) =>
                        c.Border(1)
                         .BorderColor("#DDD")
                         .Padding(4)
                         .AlignMiddle()
                         .AlignLeft();

                    string[] headers =
                    [
                        "Customer", "Issue Date", "Due Date",
                        "Item List", "Amount", "Currency",
                        "Payment Status", "Client", "Bank Acc. #", "Total Price"
                    ];

                    foreach (var h in headers)
                    {
                        table.Cell().Element(Cell).Text(h).Bold();
                    }

                    string itemList = string.Join("\n",
                        invoice.Items.Select(i => $"{i.Name} — {i.Price:N2}"));

                    string[] row =
                    [
                        invoice.CustomerId ?? "—",
                        invoice.IssueDate.ToString("yyyy‑MM‑dd"),
                        invoice.DueDate.ToString("yyyy‑MM‑dd"),
                        itemList,
                        invoice.Amount.ToString("N2"),
                        invoice.CurrencyId ?? "—",
                        invoice.PaymentStatus ?? "—",
                        invoice.ClientId ?? "—",
                        invoice.BankAccountNumber ?? "—",
                        invoice.Amount.ToString("N2")
                    ];

                    foreach (var cell in row)
                    {
                        table.Cell().Element(Cell).Text(cell);
                    }
                });

                // ---- Footer ----
                page.Footer()
                    .AlignCenter()
                    .Text($"Generated {DateTime.UtcNow:u}").Italic();
            });
        });

        return doc.GeneratePdf();
    }
}
