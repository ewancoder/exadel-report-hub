using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.Models.Models;

public sealed class Customer : AuditModel, IModel
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId CountryId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public ObjectId Id { get; set; }
}
