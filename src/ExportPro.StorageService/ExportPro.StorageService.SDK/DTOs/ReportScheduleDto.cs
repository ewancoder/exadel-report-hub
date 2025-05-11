using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs;

public sealed class ReportScheduleDto
{
    public ReportFrequency Frequency { get; set; } = ReportFrequency.Weekly;
    public DayOfWeek? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public TimeOnly Time { get; set; } = new(8, 0);
}