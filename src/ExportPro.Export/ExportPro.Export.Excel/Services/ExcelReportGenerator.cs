using ClosedXML.Excel;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

namespace ExportPro.Export.Excel.Services;

public sealed class ExcelReportGenerator : IReportGenerator
{
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string Extension => "xlsx";

    public byte[] Generate(StatisticsReportDto data)
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

    private static void GenerateReportInfoSheet(StatisticsReportDto data, XLWorkbook wb)
    {
        var info = wb.Worksheets.Add("ReportInfo");
        info.Cell("A1").Value = "GeneratedAt"; info.Cell("B1").Value = DateTime.UtcNow.ToString("u");
        info.Cell("A2").Value = "ClientId"; info.Cell("B2").Value = data.Filters.ClientId ?? "—";
    }

    private static void GeneratePlansSheet(StatisticsReportDto data, XLWorkbook wb)
    {
        wb.Worksheets.Add("Plans")
          .Cell(1, 1)
          .InsertTable(data.Plans, "Plans", true);
    }

    private static void GenerateItemsSheet(StatisticsReportDto data, XLWorkbook wb)
    {
        wb.Worksheets.Add("Items")
          .Cell(1, 1)
          .InsertTable(data.Items, "Items", true);
    }

    private static void GenerateInvoicesSheet(StatisticsReportDto data, XLWorkbook wb)
    {
        wb.Worksheets.Add("Invoices")
          .Cell(1, 1)
          .InsertTable(ProjectInvoices(data.Invoices), "Invoices", true);
    }

    private static IEnumerable<object> ProjectInvoices(IEnumerable<InvoiceDto> src) =>
        src.Select(i => new
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
