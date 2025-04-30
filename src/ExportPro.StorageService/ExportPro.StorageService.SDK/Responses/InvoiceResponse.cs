using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class InvoiceResponse
{
    public required Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public double Amount { get; set; }
    public string CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public string CustomerId { get; set; }
    public string BankAccountNumber { get; set; }
    public string ClientId { get; set; }
    public List<ItemResponse> Items { get; set; }
}
