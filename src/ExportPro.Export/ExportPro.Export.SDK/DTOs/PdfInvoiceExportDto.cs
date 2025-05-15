namespace ExportPro.Export.SDK.DTOs;

public sealed class PdfInvoiceExportDto
{
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public string? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ClientName { get; set; }
    public string? CustomerName { get; set; }
    public List<PdfItemExportDto> Items { get; set; } = [];
}