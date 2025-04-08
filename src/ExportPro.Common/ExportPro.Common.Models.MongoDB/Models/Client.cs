using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.Common.Models.MongoDB.Models;

public class Client
{
    [BsonId]
    public ObjectId ClientId { get; set; } 
    public string Name { get; set; }
    public ICollection<ObjectId> InvoiceIds { get; set; }
}