namespace ExportPro.Export.SDK.Utilities;

public static class FileNameTemplates
{
    public static string InvoicePdfFileName(string number)
    {
        return $"invoice_{number}.pdf";
    }

    public static string CsvExcelFileName(string ext)
    {
        return $"statistics_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{ext}";
    }
}