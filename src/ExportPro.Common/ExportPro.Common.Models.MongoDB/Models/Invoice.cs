using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.Common.Models.MongoDB.Models;

public class Invoice
{
    [BsonId]
    public ObjectId InvoceId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; } 
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentStatus { get; set; }
    public string InvoiceNumber { get; set; }
    public string BankAccountNumber { get; set; }
    public string Status { get; set; }
    public ObjectId ClientId { get; set; }
}