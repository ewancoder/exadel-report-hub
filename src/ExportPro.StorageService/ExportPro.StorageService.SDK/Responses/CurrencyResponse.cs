namespace ExportPro.StorageService.SDK.Responses;

public sealed class CurrencyResponse
{
    public required Guid Id { get; set; }
    public required string CurrencyCode { get; set; } // e.g. "USD"
}
