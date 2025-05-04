using ClosedXML.Excel;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

namespace ExportPro.Export.Excel.Services;

public sealed class ExcelReportGenerator : IReportGenerator
{
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string Extension => "xlsx";

    public byte[] Generate(ReportContentDto data)
    {
        using var wb = new XLWorkbook();
        GenerateInvoicesSheet(data, wb);
        GenerateItemsSheet(data, wb);
        GeneratePlansSheet(data, wb);
        GenerateReportInfoSheet(data, wb);
        return FinalizeExcelData(wb);
    }

    private static byte[] FinalizeExcelData(XLWorkbook wb)
    {
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static void GenerateReportInfoSheet(ReportContentDto data, XLWorkbook wb)
    {
        var sht = wb.Worksheets.Add("ReportInfo");

        sht.Cell("A1").Value = "GeneratedAt";
        sht.Cell("B1").Value = DateTime.UtcNow.ToString("u");

        sht.Cell("A2").Value = "ClientIds";
        sht.Cell("B2").Value =
            string.Join(", ", data.ClientNames.Select(kv => $"{kv.Value} ({kv.Key})"));

        sht.Cell("A3").Value = "IssueDateFrom";
        sht.Cell("B3").Value = data.Filters.IssueDateFrom?.ToString("u") ?? "—";
    }

    private static void GeneratePlansSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Plans");
        var row = 1;

        foreach (var (clientId, plans) in data.PlansByClient)
        {
            data.ClientNames.TryGetValue(clientId, out var cName);
            ws.Cell(row, 1).Value = $"Client: {cName ?? "—"} ({clientId})";
            ws.Cell(row, 1).Style.Font.SetBold();
            row++;

            var tbl = ws.Cell(row, 1).InsertTable(plans, $"Pl_{clientId}", true);
            row += tbl.RowCount() + 2;
        }
    }

    private static void GenerateItemsSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Items");
        var row = 1;

        foreach (var (clientId, items) in data.ItemsByClient)
        {
            data.ClientNames.TryGetValue(clientId, out var cName);
            ws.Cell(row, 1).Value = $"Client: {cName ?? "—"} ({clientId})";
            ws.Cell(row, 1).Style.Font.SetBold();
            row++;

            var tbl = ws.Cell(row, 1).InsertTable(items, $"It_{clientId}", true);
            row += tbl.RowCount() + 2;
        }
    }

    private static void GenerateInvoicesSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Invoices");
        var row = 1;

        foreach (var grp in data.Invoices
                     .Where(i => i.ClientId.HasValue)
                     .GroupBy(i => i.ClientId!.Value))
        {
            data.ClientNames.TryGetValue(grp.Key, out var cName);
            ws.Cell(row, 1).Value = $"Client: {cName ?? "—"} ({grp.Key})";
            ws.Cell(row, 1).Style.Font.SetBold();
            row++;

            var tbl = ws.Cell(row, 1)
                .InsertTable(ProjectInvoices(grp), $"Inv_{grp.Key}", true);

            row += tbl.RowCount() + 2;
        }
    }

    private static IEnumerable<object> ProjectInvoices(IEnumerable<InvoiceDto> src)
        => src.Select(i => new
        {
            i.Id,
            i.InvoiceNumber,
            IssueDate = i.IssueDate.ToString("yyyy-MM-dd"),
            DueDate = i.DueDate.ToString("yyyy-MM-dd"),
            i.Amount,
            i.CurrencyId,
            i.PaymentStatus,
            i.BankAccountNumber,
            i.ClientId,
            i.CustomerId
        });
}