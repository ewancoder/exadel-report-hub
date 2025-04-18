using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Commands.InvoiceCommands;

public class UpdateInvoiceCommand : ICommand<Invoice>
{
    public string Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string? CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ClientId { get; set; }
    public List<string>? ItemIds { get; set; }
}