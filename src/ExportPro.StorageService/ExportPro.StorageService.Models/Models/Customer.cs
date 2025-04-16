using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.Models.Models;

public class Customer : IModel
{
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? CountryId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }