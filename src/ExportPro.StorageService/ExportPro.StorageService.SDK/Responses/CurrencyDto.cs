namespace ExportPro.StorageService.SDK.Responses;

public class CurrencyDto
{
    public string? Id { get; set; }
    public required string CurrencyCode { get; set; }   // e.g. "USD"
}
