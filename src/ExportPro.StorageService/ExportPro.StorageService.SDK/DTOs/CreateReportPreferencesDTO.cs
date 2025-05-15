using System.Text.Json.Serialization;
using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs;

public record CreateReportPreferencesDTO
{
    public required string Email { get; set; }
    public Guid ClientId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReportFormat ReportFormat { get; set; }
    public Guid ClientCurrencyId { get; set; }
    public required ReportScheduleDto Schedule { get; set; }
}
