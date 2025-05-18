namespace ExportPro.Front.Models;

public sealed class ClientResponse
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
