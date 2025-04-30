using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public double? Amount { get; set; }
    public Guid? CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? CustomerId { get; set; }
    public List<ItemDtoForClient>? Items { get; set; }
}
