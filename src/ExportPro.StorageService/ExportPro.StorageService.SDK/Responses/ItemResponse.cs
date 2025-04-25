using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.SDK.Responses;

public class ItemResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? CustomerId { get; set; }
    public Status? Status { get; set; }
    public string? CurrencyId { get; set; } //maybe can be made into enum as well?
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
