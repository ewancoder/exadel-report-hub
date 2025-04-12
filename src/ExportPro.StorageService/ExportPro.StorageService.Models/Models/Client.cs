using ExportPro.Common.Models.MongoDB;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.Models.Models;

public class Client:IModel
{
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }=false;
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? InvoiceIds { get; set; } = new();
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? CustomerIds { get; set; } = new();
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? ItemIds { get; set; } = new();

}