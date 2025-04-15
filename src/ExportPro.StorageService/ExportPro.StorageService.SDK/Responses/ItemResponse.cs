using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ExportPro.StorageService.SDK.Responses;

public class ItemResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CustomerId { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ClientId { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? InvoiceId { get; set; }
    public Status? Status { get; set; }
    public Currency? Currency { get; set; } //maybe can be made into enum as well?
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
