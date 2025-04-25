using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.SDK.Services;

public interface ICurrencyExchangeService
{
    Task<double> ExchangeRate(CurrencyExchangeModel currenyExchangeModel,CancellationToken cancellationToken=default);
    Task<bool> DateExists(string from,string date);
}