namespace ExportPro.Front.Models;
public sealed class ItemResponse
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public Guid? CustomerId { get; set; }
    public Status? Status { get; set; }
    public Guid CurrencyId { get; set; }
}

