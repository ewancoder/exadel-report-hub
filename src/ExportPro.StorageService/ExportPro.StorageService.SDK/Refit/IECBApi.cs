using System.Xml;
using System.Xml.Linq;
using Refit;
namespace ExportPro.StorageService.SDK.Refit;

public interface IECBApi
{
    [Get("/D.{from}.{to}.SP00.A?startPeriod={date}&endPeriod={date}")]
    Task<XmlDocument> GetXmlDocument([AliasAs("from")] string from,
            [AliasAs("to")] string to,
            [AliasAs("date")] string date);
}
