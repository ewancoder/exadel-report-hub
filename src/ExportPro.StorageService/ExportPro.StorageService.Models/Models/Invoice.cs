using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.Models.Models;

public sealed class Invoice : AuditModel, IModel
{
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now;
    public double? Amount { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId CurrencyId { get; set; }

    public ObjectId ClientCurrencyId { get; set; }

    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId ClientId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId CustomerId { get; set; }

    public List<Item>? Items { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ObjectId Id { get; set; }
}
