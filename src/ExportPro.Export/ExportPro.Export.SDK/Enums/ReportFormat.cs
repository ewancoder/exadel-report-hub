using System.Text.Json.Serialization;

namespace ExportPro.Export.SDK.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReportFormat
{
    Csv,
    Xlsx
}