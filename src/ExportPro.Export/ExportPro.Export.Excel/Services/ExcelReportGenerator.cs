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
        var info = wb.Worksheets.Add("ReportInfo");
        info.Cell("A1").Value = "GeneratedAt";
        info.Cell("B1").Value = DateTime.UtcNow.ToString("u");
        info.Cell("A2").Value = "Client:";
        info.Cell("B2").Value = data.ClientName;
    }

    private static void GeneratePlansSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Plans");
        ws.Cell(1, 1).Value = $"Client: {data.ClientName}";
        ws.Cell(1, 1).Style.Font.SetBold();
        ws.Cell(2, 1).InsertTable(
            data.Plans.Select(p => new
            {
                p.StartDate,
                p.EndDate,
                p.Amount,
                p.CreatedAt,
                p.UpdatedAt
            }),
            "Plans",
            true
        );
    }

    private static void GenerateItemsSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Items");
        ws.Cell(1, 1).Value = $"Client: {data.ClientName}";
        ws.Cell(1, 1).Style.Font.SetBold();
        ws.Cell(2, 1).InsertTable(
            data.Items.Select(i => new
            {
                i.Name,
                i.Description,
                i.Price,
                i.Status,
                Currency = data.CurrencyCodes.GetValueOrDefault(i.CurrencyId, "—"),
                i.CreatedAt,
                i.UpdatedAt
            }),
            "Items",
            true
        );
    }

    private static void GenerateInvoicesSheet(ReportContentDto data, XLWorkbook wb)
    {
        var ws = wb.Worksheets.Add("Invoices");
        ws.Cell(1, 1).Value = $"Client: {data.ClientName}";
        ws.Cell(1, 1).Style.Font.SetBold();
        ws.Cell(2, 1).InsertTable(ProjectInvoices(data), "Invoices", true);

        // >>> append overdue summary after two blank rows
        var startRow = ws.LastRowUsed()!.RowNumber() + 3;
        ws.Cell(startRow, 1).Value = "Overdue Invoices";
        ws.Cell(startRow, 1).Style.Font.SetBold();

        ws.Cell(startRow + 1, 1).Value = "Count";
        ws.Cell(startRow + 1, 2).Value = "Amount";
        ws.Cell(startRow+1,3).Value = "Client Currency";
        ws.Cell(startRow + 2, 1).Value = data.OverdueInvoicesCount;
        ws.Cell(startRow + 2, 2).Value = (data.TotalOverdueAmount.HasValue
            ? data.TotalOverdueAmount.Value.ToString("N2")
            : "—") ;
        ws.Cell(startRow+2,3).Value = data.ClientCurrencyCode;
    }

    private static IEnumerable<object> ProjectInvoices(ReportContentDto data)
    {
        var dict = data.CurrencyCodes;
        return data.Invoices.Select(i => new
        {
            i.InvoiceNumber,
            IssueDate = i.IssueDate.ToString("yyyy-MM-dd"),
            DueDate = i.DueDate.ToString("yyyy-MM-dd"),
            i.Amount,
            Currency = i.CurrencyId.HasValue
                ? dict.GetValueOrDefault(i.CurrencyId.Value, "—")
                : "—",
            i.PaymentStatus,
            i.BankAccountNumber
        });
    }
}