namespace ExportPro.StorageService.SDK.Responses;

public class RestCountries
{
    public NameDto? Name { get; set; }
    public string? Cioc { get; set; }
    public Dictionary<string, CurrencyDtoFromApi>? Currencies { get; set; }
}

public class NameDto
{
    public string? Common { get; set; }
    public string? Official { get; set; }
    public Dictionary<string, NativeNameDto>? NativeName { get; set; }
}

public class NativeNameDto
{
    public string? Official { get; set; }
    public string? Common { get; set; }
}

public class CurrencyDtoFromApi
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }
}
