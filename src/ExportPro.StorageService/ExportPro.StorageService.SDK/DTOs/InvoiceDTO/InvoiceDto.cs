using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

public sealed class InvoiceDto : AuditModel
{
    public Guid Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public double? Amount { get; set; }
    public required string Currency { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public required Guid ClientId { get; set; }
    public required Guid CustomerId { get; set; }
    public List<ItemDtoForInvoice>? Items { get; set; }
}
