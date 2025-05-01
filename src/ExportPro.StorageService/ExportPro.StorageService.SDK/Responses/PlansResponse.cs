namespace ExportPro.StorageService.SDK.Responses;

public sealed class PlansResponse
{
    public required Guid Id { get; set; }
    public int Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required List<ItemResponse> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
