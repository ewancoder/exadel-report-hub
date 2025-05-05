
using System.Xml;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;

namespace ExportPro.StorageService.SDK.Services;

public sealed class CurrencyExchangeService(IECBApi ecbApi) : ICurrencyExchangeService
{
   public async Task<double> ExchangeRate(
    CurrencyExchangeModel currenyExchangeModel,
    CancellationToken cancellationToken = default)
    {
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

    private async Task<DateTime> GetLastValidDateAsync(string currencyCode, DateTime date)
    {
        var currentDate = date;

        // if weekend, go back to Friday
        if (currentDate.DayOfWeek == DayOfWeek.Saturday)
            currentDate = currentDate.AddDays(-1);
        else if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            currentDate = currentDate.AddDays(-2);

        // check for holidays by verifying data exists
        for (int attempts = 0; attempts < 7; attempts++)
        {
            var formatted = currentDate.ToString("yyyy-MM-dd");
            if (await DateExists(currencyCode, formatted))
                return currentDate;

            currentDate = currentDate.AddDays(-1);
        }

        throw new InvalidOperationException("Could not find a valid exchange rate date within 7 days.");
    }
}
