using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Profiles.CustomerMaps;

public static class CustomerMapper
{
    public static CustomerDto ToDto(Customer customer) => new()
    {
        Id = customer.Id.ToString(),
        Name = customer.Name,
        Email = customer.Email,
        CountryId = customer.CountryId,
        IsDeleted = customer.IsDeleted,
        CreatedAt = customer.CreatedAt,
        UpdatedAt = customer.UpdatedAt ?? DateTime.MinValue
    };

    public static Customer ToEntity(CustomerDto customer) => new()
    {
        Id = ObjectId.Parse(customer.Id),
        Name = customer.Name,
        Email = customer.Email,
        CountryId = customer.CountryId,
        IsDeleted = customer.IsDeleted,
        CreatedAt = customer.CreatedAt,
        UpdatedAt = customer.UpdatedAt ?? DateTime.MinValue
    };
}