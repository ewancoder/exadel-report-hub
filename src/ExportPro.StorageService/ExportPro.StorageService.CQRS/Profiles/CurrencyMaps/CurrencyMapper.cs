using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Profiles.CurrencyMaps;

public static class CurrencyMapper
{
    public static CurrencyDto ToDto(Currency currency) => new()
    {
        Id = currency.Id.ToString(),
        CurrencyCode = currency.CurrencyCode
    };

    public static Currency ToEntity(CurrencyDto currency) => new()
    {
        Id = ObjectId.Parse(currency.Id),
        CurrencyCode = currency.CurrencyCode
    };
}