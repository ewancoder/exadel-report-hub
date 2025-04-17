using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.SDK.DTOs;

public class ClientUpdateDto
{
    [BsonIgnoreIfNull]
    public string? Name { get; set; }
    [BsonIgnoreIfNull]
    public string? Description { get; set; }
   
}