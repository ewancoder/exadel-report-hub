using ExportPro.StorageService.Models.Enums;

namespace ExportPro.Export.SDK.DTOs;

public class InvoiceForReport
{
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public double? Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
}
