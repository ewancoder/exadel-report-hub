using System.Xml;
using System.Xml.Linq;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface IECBApi
{
    [Get("/D.{from}.EUR.SP00.A?startPeriod={date}&endPeriod={date}")]
    Task<XmlDocument> GetXmlDocument(
        [AliasAs("from")] string from,
        [AliasAs("date")] string date
    );

    [Get("/D.{from}.EUR.SP00.A?startPeriod={date}&endPeriod={date}")]
    Task<HttpResponseMessage> DataExists(
        [AliasAs("from")] string from,
        [AliasAs("date")] string date
    );
}
