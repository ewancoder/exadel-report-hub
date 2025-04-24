using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Profiles.CountryMaps;

public static class CountryMapper
{
    public static CountryDto ToDto(Country country)
    {
        return new CountryDto
        {
            Id = country.Id.ToString(),
            Name = country.Name,
            Code = country.Code,
            CurrencyId = country.CurrencyId,
            IsDeleted = country.IsDeleted,
            CreatedAt = country.CreatedAt,
            UpdatedAt = country.UpdatedAt
        };
    }

    public static Country ToEntity(CountryDto country)
    {
        return new Country
        {
            Id = ObjectId.Parse(country.Id),
            Name = country.Name,
            Code = country.Code,
            CurrencyId = country.CurrencyId,
            IsDeleted = country.IsDeleted,
            CreatedAt = country.CreatedAt,
            UpdatedAt = country.UpdatedAt
        };
    }
}