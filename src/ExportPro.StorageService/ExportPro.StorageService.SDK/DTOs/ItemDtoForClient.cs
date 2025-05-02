using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs;

public sealed class ItemDtoForClient
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required double Price { get; set; }
    public Status Status { get; set; }
    public Guid CurrencyId { get; set; } //maybe can be made into enum as well?
}
