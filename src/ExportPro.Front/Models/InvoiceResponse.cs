﻿namespace ExportPro.Front.Models;
public sealed class InvoiceResponse
{
    public required Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public double Amount { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid ClientCurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public Guid CustomerId { get; set; }
    public string BankAccountNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public required List<ItemResponse> Items { get; set; }
}
