using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.Common.Models.MongoDB.Models;

public class Client:IModel
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<ObjectId>? InvoiceIds { get; set; }
}