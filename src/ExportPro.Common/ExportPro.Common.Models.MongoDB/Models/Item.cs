using ExportPro.Common.Models.MongoDB.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ExportPro.Common.Models.MongoDB.Models;

public class Item : IModel
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } 
    public double Price { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string CustomerId { get; set; }
    public Status Status { get; set; }
    public string Currency {  get; set; } //maybe can be made into enum as well?

}

