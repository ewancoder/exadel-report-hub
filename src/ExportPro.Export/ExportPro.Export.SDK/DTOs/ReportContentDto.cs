using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.Export.SDK.DTOs;

public sealed record ReportContentDto
{
    public List<InvoiceDto> Invoices { get; init; } = [];
    public List<ItemResponse> Items { get; init; } = [];
    public List<PlansResponse> Plans { get; init; } = [];
    public string ClientName { get; init; } = "—";
    public ReportFilterDto Filters { get; init; } = new();
    public int OverdueInvoicesCount { get; init; }
    public string ClientCurrencyCode { get; init; } = "—";
    public double? TotalOverdueAmount   { get; init; }
    public Dictionary<Guid, string> CurrencyCodes { get; init; } = [];
}