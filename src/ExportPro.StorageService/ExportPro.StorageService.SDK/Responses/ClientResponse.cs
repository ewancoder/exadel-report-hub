using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class ClientResponse : AuditModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
