using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class ReportPreferenceResponse
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
    public required Guid ClientId { get; set; }
    public ReportFormat ReportFormat { get; set; }
    public required string CronExpression { get; set; }
    public string? HumanReadableSchedule { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public required string CreatedBy { get; set; }
    public bool IsEnabled { get; set; }
}