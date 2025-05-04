using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.Export.SDK.DTOs;

public sealed class ReportContentDto
{
    public List<InvoiceDto> Invoices { get; init; } = [];
    public List<ItemResponse> Items { get; init; } = [];
    public List<PlansResponse> Plans { get; init; } = [];
    public Dictionary<Guid, List<ItemResponse>> ItemsByClient { get; init; } = [];
    public Dictionary<Guid, List<PlansResponse>> PlansByClient { get; init; } = [];
    public ReportFilterDto Filters { get; init; } = new();
}