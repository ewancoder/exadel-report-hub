namespace ExportPro.Export.SDK.Utilities;

public static class ReportFileNameTemplates
{
    public static string Statistics(string ext) =>
        $"statistics_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{ext}";
}