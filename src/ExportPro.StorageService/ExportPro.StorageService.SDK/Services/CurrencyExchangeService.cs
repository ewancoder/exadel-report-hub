using System.Xml;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;

namespace ExportPro.StorageService.SDK.Services;

public sealed class CurrencyExchangeService(IECBApi ecbApi) : ICurrencyExchangeService
{
    public async Task<double> ExchangeRate(
        CurrencyExchangeModel currenyExchangeModel,
        CancellationToken cancellationToken = default
    )
    {
        if (currenyExchangeModel.From is not null && currenyExchangeModel.To is not null)
            currenyExchangeModel.From = currenyExchangeModel.To;
        if (currenyExchangeModel.From == "EUR")
            return 1.0;
        var validDate = await GetLastValidDateAsync(currenyExchangeModel.From, currenyExchangeModel.Date);
        var dateString = validDate.ToString("yyyy-MM-dd");

        var xmlDocument = await ecbApi.GetXmlDocument(currenyExchangeModel.From, dateString);
        var namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
        namespaceManager.AddNamespace("generic", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/data/generic");

        var valueNode = xmlDocument.SelectSingleNode("//generic:ObsValue", namespaceManager);
        var rateString = valueNode?.Attributes?["value"]?.Value;

        if (!double.TryParse(rateString, out var rate))
            throw new InvalidOperationException("Could not parse exchange rate.");

        return rate;
    }

    //checks if the date is holiday
    public async Task<bool> DateExists(string from, string date)
    {
        var response = await ecbApi.DataExists(from, date);
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
            return false;
        return true;
    }

    public async Task<double> ConvertTwoCurrencies(
        CurrencyExchangeModel currenyExchangeModel,
        CancellationToken cancellationToken = default
    )
    {
        if (currenyExchangeModel.From == currenyExchangeModel.To)
            return (double)currenyExchangeModel.AmountFrom;
        var exchangeRateToEuroFromSrcCurrency = 1.0;
        var exchangeRateToEuroFromDestCurrency = 1.0;
        if (currenyExchangeModel.From != "EUR")
        {
            var currencyMoodel = new CurrencyExchangeModel
            {
                Date = currenyExchangeModel.Date,
                From = currenyExchangeModel.From,
                To = "EUR",
                AmountFrom = currenyExchangeModel.AmountFrom,
            };
            exchangeRateToEuroFromSrcCurrency = await ExchangeRate(currencyMoodel, cancellationToken);
        }
        if (currenyExchangeModel.To != "EUR")
            exchangeRateToEuroFromDestCurrency = await ExchangeRate(currenyExchangeModel, cancellationToken);
        var amount =
            currenyExchangeModel.AmountFrom * exchangeRateToEuroFromDestCurrency / exchangeRateToEuroFromSrcCurrency;
        return (double)amount!;
    }

    private async Task<DateTime> GetLastValidDateAsync(string currencyCode, DateTime date)
    {
        var currentDate = date;

        // if weekend, go back to Friday
        if (currentDate.DayOfWeek == DayOfWeek.Saturday)
            currentDate = currentDate.AddDays(-1);
        else if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            currentDate = currentDate.AddDays(-2);

        // check for holidays by verifying data exists
        for (var attempts = 0; attempts < 7; attempts++)
        {
            var formatted = currentDate.ToString("yyyy-MM-dd");
            if (await DateExists(currencyCode, formatted))
                return currentDate;

            currentDate = currentDate.AddDays(-1);
        }

        throw new InvalidOperationException("Could not find a valid exchange rate date within 7 days.");
    }
}
