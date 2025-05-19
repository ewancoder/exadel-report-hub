using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

public class CreateInvoiceDto
{
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public Status? PaymentStatus { get; set; }
    public Guid CustomerId { get; set; }
    public string? BankAccountNumber { get; set; }
    public Guid ClientId { get; set; }
    public List<Guid>? Items { get; set; }
}
