using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class ItemResponse : AuditModel
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public Guid? CustomerId { get; set; }
    public Status? Status { get; set; }
    public string Currency { get; set; } //maybe can be made into enum as well?
}
