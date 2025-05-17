namespace ExportPro.StorageService.Models.Models;

public sealed class CurrencyExchangeModel
{
    public required string From { get; set; }
    public string? To { get; set; }
    public double? AmountFrom { get; set; }
    public required DateTime Date { get; set; }
}
