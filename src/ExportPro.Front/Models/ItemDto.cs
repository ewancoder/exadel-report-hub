namespace ExportPro.Front.Models;
public class ItemDto
{
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid? CustomerId { get; set; }
    public Status Status { get; set; } = Status.Unpaid;
    public string? Currency { get; set; }
}
