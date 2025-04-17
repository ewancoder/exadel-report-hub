namespace ExportPro.StorageService.SDK.Responses;

public class PlansResponse
{
    public string Id { get; set; }
    public int Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ItemResponse> Items { get; set; }
}