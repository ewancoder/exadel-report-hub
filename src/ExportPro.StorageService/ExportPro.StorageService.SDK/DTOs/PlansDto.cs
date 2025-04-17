namespace ExportPro.StorageService.SDK.DTOs;

public class PlansDto
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; }
    public List<ItemDtoForClient> Items { get; set; }
}
