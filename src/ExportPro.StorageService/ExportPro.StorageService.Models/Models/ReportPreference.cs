using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class ReportPreference : IModel
{
    public ObjectId Id { get; set; }
    
    public required ObjectId UserId { get; set; }
    
    public required string Email { get; set; }

    public required ObjectId ClientId { get; set; }

    public ReportFormat ReportFormat { get; set; } = ReportFormat.Csv;
    
    public required string CronExpression { get; set; }

    public bool IsDelivered { get; set; }

    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    public  string jwtToken { get; set; } 

    public bool IsDeleted { get; set; } = false;    
}