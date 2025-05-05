using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class ReportPreferenceResponse
{
    public required Guid Id { get; set; } 
    public required Guid UserId { get; set; } 
    public required Guid ClientId { get; set; }
    public ReportFrequency Frequency { get; set; }
    public TimeOnly TimeToSend { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public ReportFormat ReportFormat { get; set; }
    public bool IsEnabled { get; set; }
}