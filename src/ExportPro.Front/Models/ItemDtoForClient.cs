namespace ExportPro.Front.Models;
public sealed class ItemDtoForClient
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required double Price { get; set; }
    public Status Status { get; set; }
    public Guid CurrencyId { get; set; } 
    public string CurrencyName { get; set; } = string.Empty;
}

