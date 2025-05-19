namespace ExportPro.StorageService.SDK.DTOs;

public class TotalInvoicesDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid ClientId { get; set; }
}
