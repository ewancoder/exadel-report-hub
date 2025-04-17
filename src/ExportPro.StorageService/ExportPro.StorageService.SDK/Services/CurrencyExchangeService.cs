
using System.Xml;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;

namespace ExportPro.StorageService.SDK.Services;

public  class CurrencyExchangeService(IECBApi ecbApi) : ICurrencyExchangeService
{
    private readonly IECBApi _ecbApi = ecbApi;

    public async Task<double> ConvertCurrency(CurrenyExchangeModel currenyExchangeModel)
    {
        if(currenyExchangeModel.AmountFrom == 0)
        {
            return 0;
        }
        var exchangeRate = await ExchangeRate(currenyExchangeModel);
        if (exchangeRate == 0)
        {
            return 0;
        }
        var amountTo = currenyExchangeModel.AmountFrom * exchangeRate;
        return amountTo ?? 0;
    }

    public async Task<double> ExchangeRate(CurrenyExchangeModel currenyExchangeModel)
    {
        var day = currenyExchangeModel.Date.ToString("dddd");
        if (day == "Saturday")
        {
            currenyExchangeModel.Date = currenyExchangeModel.Date.AddDays(-1);
        }
        if (day == "Sunday")
        {
            currenyExchangeModel.Date =  currenyExchangeModel.Date.AddDays(-2);
        }
        var dateCorrectFormat = currenyExchangeModel.Date.ToString("yyyy-MM-dd");    
        var xmlDocument = await _ecbApi.GetXmlDocument(currenyExchangeModel.From, currenyExchangeModel.To, dateCorrectFormat);
        var namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
        namespaceManager.AddNamespace("generic", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/data/generic");
        var value = xmlDocument.SelectSingleNode("//generic:ObsValue", namespaceManager);
        var currencyValue = value.Attributes["value"]?.Value;
        return currencyValue != null ? Convert.ToDouble(currencyValue) : 0;
    }

}
