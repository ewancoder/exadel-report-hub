namespace ExportPro.StorageService.SDK.DTOs.CountryDTO;

public class CountryDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsDeleted { get; set; }
    public string? CurrencyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
