using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.SDK.Responses;

public class ClientResponse
{
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } 
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public List<ItemResponse> itemResponses { get; set; }
}
