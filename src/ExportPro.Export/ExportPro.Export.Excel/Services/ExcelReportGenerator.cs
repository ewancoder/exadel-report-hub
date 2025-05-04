using ClosedXML.Excel;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

namespace ExportPro.Export.Excel.Services;

public sealed class ExcelReportGenerator : IReportGenerator
{
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string Extension => ReportFileTypes.Excel;

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

    private static void GenerateInvoicesSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Invoices");
        int row = 1;

        foreach (var grp in data.Invoices.GroupBy(i => i.ClientId))
        {
            ws.Cell(row, 1).Value = $"ClientId: {grp.Key}";
            ws.Cell(row, 1).Style.Font.SetBold();
            row++;

            var tbl = ws.Cell(row, 1)
                .InsertTable(ProjectInvoices(grp), $"Inv_{grp.Key}", true);
            row += tbl.RowCount() + 2;
        }
    }

    private static void GenerateItemsSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Items");
        int row = 1;

        foreach (var (clientId, items) in data.ItemsByClient)
        {
            ws.Cell(row, 1).Value = $"ClientId: {clientId}";
            ws.Cell(row, 1).Style.Font.SetBold();
            row++;

            var tbl = ws.Cell(row, 1).InsertTable(items, $"It_{clientId}", true);
            row += tbl.RowCount() + 2;
        }
    }

    private static void GeneratePlansSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Plans");
        int row = 1;

        foreach (var (clientId, plans) in data.PlansByClient)
        {
            ws.Cell(row, 1).Value = $"ClientId: {clientId}";
            ws.Cell(row, 1).Style.Font.SetBold();
            row++;

            var tbl = ws.Cell(row, 1).InsertTable(plans, $"Pl_{clientId}", true);
            row += tbl.RowCount() + 2;
        }
    }

    private static void GenerateReportInfoSheet(ReportContentDto data, XLWorkbook wb)
    {
        var info = wb.Worksheets.Add("ReportInfo");
        info.Cell("A1").Value = "GeneratedAt";
        info.Cell("B1").Value = DateTime.UtcNow.ToString("u");

        info.Cell("A2").Value = "ClientIds";
        info.Cell("B2").Value = string.Join(", ", data.Filters.ClientIds ?? []);

        info.Cell("A3").Value = "IssueDateFrom";                       // NEW
        info.Cell("B3").Value = data.Filters.IssueDateFrom?.ToString("u") ?? "—"; // NEW
    }

    private static IEnumerable<object> ProjectInvoices(IEnumerable<InvoiceDto> src)
    {
        return src.Select(i => new
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
}