namespace ExportPro.StorageService.Models.Models;

public class CurrencyExchangeModel
{
    public required string From { get; set; }
    public required DateTime Date { get; set; }
    public double? AmountFrom { get; set; }
}
