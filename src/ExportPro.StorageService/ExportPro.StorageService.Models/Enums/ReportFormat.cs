using System.Text.Json.Serialization;

namespace ExportPro.StorageService.Models.Enums;

// [JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReportFormat
{
    Csv,
    Xlsx,
}
