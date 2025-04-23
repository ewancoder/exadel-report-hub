namespace ExportPro.Export.SDK.DTOs;

public sealed class PdfInvoiceExportDto
{
    public string Id { get; set; } = default!;
    public string InvoiceNumber { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string? CurrencyId { get; set; }
    public string? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ClientId { get; set; }
    public string? CustomerId { get; set; }

    public List<PdfItemExportDto> Items { get; set; } = [];
}
