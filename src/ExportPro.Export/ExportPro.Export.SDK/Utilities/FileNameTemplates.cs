namespace ExportPro.Export.SDK.Utilities;

public static class FileNameTemplates
{
    public static string InvoicePdfFileName(string number) => $"invoice_{number}.pdf";
}