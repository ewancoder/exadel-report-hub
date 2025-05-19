namespace ExportPro.Front.Models;

public sealed class ClientResponse
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
}
