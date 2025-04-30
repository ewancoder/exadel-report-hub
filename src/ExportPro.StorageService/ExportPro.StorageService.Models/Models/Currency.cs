using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class Currency : IModel
{
    public ObjectId Id { get; set; }
    public required string CurrencyCode { get; set; } // e.g. "USD"
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
