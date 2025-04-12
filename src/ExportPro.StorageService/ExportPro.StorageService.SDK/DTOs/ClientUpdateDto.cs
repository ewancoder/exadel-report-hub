using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.SDK.DTOs;

public class ClientUpdateDto
{
    [BsonIgnoreIfNull]
    public string? Name { get; set; }
    [BsonIgnoreIfNull]
    public string? Description { get; set; }
    [BsonIgnoreIfNull]
    public bool IsDeleted { get; set; } = false;
    // [BsonRepresentation(BsonType.ObjectId)]
    // [BsonIgnoreIfNull]
    // public List<string>? InvoiceIds { get; set; } 
    // [BsonRepresentation(BsonType.ObjectId)]
    // [BsonIgnoreIfNull]
    // public List<string>? CustomerIds { get; set; } 
    // [BsonRepresentation(BsonType.ObjectId)]
    // [BsonIgnoreIfNull]
    // public List<string>? ItemIds { get; set; }    
}