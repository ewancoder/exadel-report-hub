namespace ExportPro.StorageService.SDK.Responses;

public class CustomerResponse
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Guid CountryId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
