using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class ReportPreferenceResponse : AuditModel
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
    public required Guid ClientId { get; set; }
    public ReportFormat ReportFormat { get; set; }
    public required string CronExpression { get; set; }
    public string? HumanReadableSchedule { get; set; }
    public Guid ClientCurrencyId { get; set; }
    public bool IsEnabled { get; set; }
}
