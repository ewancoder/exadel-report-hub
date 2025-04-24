using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Profiles.ItemMaps;

public static class ItemMapper
{
    public static ItemResponse ToDto(Item item)
    {
        return new ItemResponse
        {
            Id = item.Id.ToString(),
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            CustomerId = item.CustomerId,
            Status = item.Status,
            CurrencyId = item.CurrencyId,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public static Item ToEntity(ItemDtoForClient item)
    {
        return new Item
        {
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            Status = item.Status,
            CurrencyId = item.CurrencyId,
        };
    }

    public static ItemDtoForClient ToEntityForClient(Item item)
    {
        return new ItemDtoForClient
        {
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            Status = item.Status,
            CurrencyId = item.CurrencyId,
        };
    }
}