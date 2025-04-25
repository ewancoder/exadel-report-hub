using System.Security.Cryptography;
using System.Xml;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using FluentValidation;

namespace ExportPro.StorageService.SDK.Services;

public class CurrencyExchangeService(IECBApi ecbApi) : ICurrencyExchangeService
{
    public async Task<double> ExchangeRate(CurrencyExchangeModel currenyExchangeModel, CancellationToken cancellationToken = default)
    {
        //getting the day of the date 
        //because if it is saturday or sunday then we need to
        //go back to the last working day because the api is resting on that day
        var day = currenyExchangeModel.Date.ToString("dddd");
        if (day == "Saturday")
        {
            currenyExchangeModel.Date = currenyExchangeModel.Date.AddDays(-1);
        }
        if (day == "Sunday")
        {
            currenyExchangeModel.Date = currenyExchangeModel.Date.AddDays(-2);
        }
        var dateCorrectFormat = currenyExchangeModel.Date.ToString("yyyy-MM-dd");
        //need to confirm that the date is not a holiday
        var data_exists = await DateExists("USD",dateCorrectFormat);
        int sub = -1;
        int ind = 7;
        // if it a holiday then we need to go back to the last working day
        while (!data_exists && ind > 0)
        {
            currenyExchangeModel.Date = currenyExchangeModel.Date.AddDays(sub);
            dateCorrectFormat = currenyExchangeModel.Date.ToString("yyyy-MM-dd");
            //checking if the date is a holiday or not
            data_exists = await DateExists("USD", dateCorrectFormat);
            if (data_exists)
            {
                break;
            }
            ind--;
            sub--;
        }
        //getting the xml document 
        var xmlDocument = await ecbApi.GetXmlDocument(
            currenyExchangeModel.From,
            dateCorrectFormat
        );
        var namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
        namespaceManager.AddNamespace("generic", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/data/generic");
        //getting the exchange rate 
        var value = xmlDocument.SelectSingleNode("//generic:ObsValue", namespaceManager);
        var currencyValue = value.Attributes?["value"]?.Value;
        return currencyValue != null ? Convert.ToDouble(currencyValue) : 0;
    }
    
    //checks if the date is holiday
    public async Task<bool> DateExists(string from, string date)
    {
        var response = await ecbApi.DataExists(from, date);
        string content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }
        return true;
    }

}