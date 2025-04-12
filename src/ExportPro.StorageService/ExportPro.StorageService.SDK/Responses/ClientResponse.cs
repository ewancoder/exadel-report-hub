using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.SDK.Responses;

public class ClientResponse
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 
    public bool IsDeleted { get; set; } = false;
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? InvoiceIds { get; set; } 
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? CustomerIds { get; set; } 
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? ItemIds { get; set; }
}
