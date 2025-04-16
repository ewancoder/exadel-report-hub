using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public class Currency : IModel
{
    public ObjectId Id { get; set; }
    public required string Name { get; set; }   // e.g. "US Dollar"
    public required string Code { get; set; }   // e.g. "USD"
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}