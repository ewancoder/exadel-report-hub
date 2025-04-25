namespace ExportPro.StorageService.SDK.Responses;

public class CurrencyResponse
{
    public string? Id { get; set; }
    public required string CurrencyCode { get; set; }   // e.g. "USD"
}
