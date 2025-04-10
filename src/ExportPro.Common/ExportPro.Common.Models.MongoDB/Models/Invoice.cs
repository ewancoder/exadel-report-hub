using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.Common.Models.MongoDB.Models;

public class Invoice : IModel
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentStatus { get; set; }
    public string BankAccountNumber { get; set; }
    public string Status { get; set; }
    [BsonRepresentation(BsonType.ObjectId)] 
    public string ClientId { get; set; }
    public List<Item> Items { get; set; }
}
