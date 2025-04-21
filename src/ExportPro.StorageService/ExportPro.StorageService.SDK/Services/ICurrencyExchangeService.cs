using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.SDK.Services;

public interface ICurrencyExchangeService
{
    Task<double> ExchangeRate(CurrenyExchangeModel currenyExchangeModel);
    Task<double> ConvertCurrency(CurrenyExchangeModel currenyExchangeModel);
}