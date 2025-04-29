namespace ExportPro.Export.SDK.Utilities;

public static class FileNameTemplates
{
    public static string InvoicePdfFileName(string number) =>
        $"invoice_{number}.pdf";

    public static string CsvExcelFileName(string ext) =>
        $"statistics_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{ext}";
}