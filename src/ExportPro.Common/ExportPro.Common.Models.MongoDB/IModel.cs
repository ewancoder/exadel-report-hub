using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.Common.Models.MongoDB;

public interface IModel
{
    [BsonId]
    ObjectId Id { get; set; }
}
