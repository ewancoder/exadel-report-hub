namespace ExportPro.Export.SDK.Utilities;

public static class ReportFileTypes
{
    public const string Csv = "csv";
    public const string Excel = "xlsx";

    public static readonly IReadOnlyList<string> All = [Csv, Excel];
}