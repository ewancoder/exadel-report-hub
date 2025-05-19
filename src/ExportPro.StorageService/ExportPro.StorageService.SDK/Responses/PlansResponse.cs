using ExportPro.Common.Models.MongoDB;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class PlansResponse : AuditModel
{
    public required Guid Id { get; set; }
    public int Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required List<ItemResponse> Items { get; set; }
}
