using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

public class ItemDtoForInvoice
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required double Price { get; set; }
    public Status Status { get; set; }
    public string? Currency { get; set; } //maybe can be made into enum as well?
}
