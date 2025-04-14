using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.SDK.Responses;

public class InvoiceResponse
{
    public string Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now;
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? ClientId { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? ItemIds { get; set; } = new();
}