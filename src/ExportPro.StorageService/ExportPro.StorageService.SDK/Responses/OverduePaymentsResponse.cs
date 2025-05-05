namespace ExportPro.StorageService.SDK.Responses;

public record OverduePaymentsResponse
{
    public int OverdueInvoicesCount { get; set; }
    public double? TotalOverdueAmount { get; set; }
}

