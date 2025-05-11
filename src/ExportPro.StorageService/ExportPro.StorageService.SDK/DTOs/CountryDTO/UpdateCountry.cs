namespace ExportPro.StorageService.SDK.DTOs.CountryDTO;

public sealed class UpdateCountry
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}