namespace ExportPro.StorageService.SDK.DTOs;

public sealed class PlansDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required List<ItemDtoForClient> Items { get; set; }
}
