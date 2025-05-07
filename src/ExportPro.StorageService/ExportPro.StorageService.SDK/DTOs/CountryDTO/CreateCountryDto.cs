namespace ExportPro.StorageService.SDK.DTOs.CountryDTO;

public class CreateCountryDto
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public required Guid CurrencyId { get; set; }
}
