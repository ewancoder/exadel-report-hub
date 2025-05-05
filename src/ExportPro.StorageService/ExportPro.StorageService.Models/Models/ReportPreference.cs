using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class ReportPreference : IModel
{
    public ObjectId Id { get; set; }
    
    public required ObjectId UserId { get; set; }
    
    public required ObjectId ClientId { get; set; }

    public ReportFormat Format { get; set; } = ReportFormat.Csv;

    public ReportFrequency Frequency { get; set; } = ReportFrequency.Weekly;

    public DayOfWeek? DayOfWeek { get; set; } // For weekly schedule (e.g., Monday)

    public int? DayOfMonth { get; set; } // For monthly schedule (e.g., 1st, 15th)

    public TimeOnly SendTime { get; set; } // Time to send (e.g., 08:00 AM)

    public bool SendToClient { get; set; } // Whether to send to client's email

    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;    
}