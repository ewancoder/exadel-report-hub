namespace ExportPro.StorageService.SDK.DTOs;

public class TotalRevenueDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required Guid ClientCurrencyId { get; set; }
}
