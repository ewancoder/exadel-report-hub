using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs;

public record UpdateReportPreferenceDTO
{
    public Guid Id { get; set; }
    public ReportFormat ReportFormat { get; set; }
    public required ReportScheduleDto Schedule { get; set; }
    public bool IsEnabled { get; set; }
}
