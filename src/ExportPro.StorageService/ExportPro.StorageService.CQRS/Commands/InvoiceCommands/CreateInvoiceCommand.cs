using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.StorageService.CQRS.Commands.InvoiceCommands;

public class CreateInvoiceCommand : ICommand<Invoice>
{
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string? CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ClientId { get; set; }
    public List<ItemDtoForClient>? Items { get; set; }
}