using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class ReportPreference : AuditModel, IModel
{
    public required ObjectId UserId { get; set; }

    public required string Email { get; set; }

    public required ObjectId ClientId { get; set; }

    public ReportFormat ReportFormat { get; set; } = ReportFormat.Csv;

    public required string CronExpression { get; set; }

    public required string HumanReadableCronExpression { get; set; }

    public bool IsDelivered { get; set; }

    public ObjectId ClientCurrencyId { get; set; }

    public bool IsEnabled { get; set; } = true;

    public bool IsDeleted { get; set; } = false;
    public ObjectId Id { get; set; }
}
