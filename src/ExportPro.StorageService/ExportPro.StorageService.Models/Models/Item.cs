using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.Models.Models;

public class Item : IModel
{
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; } 
    public double Price { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CustomerId { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public Status? Status { get; set; }
    public string? CurrencyId {  get; set; } //maybe can be made into enum as well?
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt {  get; set; }

}

