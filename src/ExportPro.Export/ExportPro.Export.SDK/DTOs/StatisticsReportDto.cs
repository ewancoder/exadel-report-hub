using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.Export.SDK.DTOs;

public class StatisticsReportDto
{
    public List<InvoiceDto> Invoices { get; init; } = [];
    public List<ItemResponse> Items { get; init; } = [];
    public List<PlansResponse> Plans { get; init; } = [];
    public StatisticsFilterDto Filters { get; init; } = new();
}
