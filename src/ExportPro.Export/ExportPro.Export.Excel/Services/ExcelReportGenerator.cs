using ClosedXML.Excel;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;

namespace ExportPro.Export.Excel.Services;

public sealed class ExcelReportGenerator : IReportGenerator
{
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string Extension => "xlsx";

    public byte[] Generate(StatisticsReportDto data)
    {
        using var wb = new XLWorkbook();

        wb.Worksheets.Add("Invoices").Cell(1, 1).InsertTable(data.Invoices, "Invoices", true);
        wb.Worksheets.Add("Items").Cell(1, 1).InsertTable(data.Items, "Items", true);
        wb.Worksheets.Add("Plans").Cell(1, 1).InsertTable(data.Plans, "Plans", true);

        var info = wb.Worksheets.Add("ReportInfo");
        info.Cell("A1").Value = "GeneratedAt"; info.Cell("B1").Value = DateTime.UtcNow.ToString("u");
        info.Cell("A2").Value = "StartDate"; info.Cell("B2").Value = data.Filters.StartDate?.ToString("yyyy-MM-dd") ?? "—";
        info.Cell("A3").Value = "EndDate"; info.Cell("B3").Value = data.Filters.EndDate?.ToString("yyyy-MM-dd") ?? "—";
        info.Cell("A4").Value = "ClientId"; info.Cell("B4").Value = data.Filters.ClientId ?? "—";

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
