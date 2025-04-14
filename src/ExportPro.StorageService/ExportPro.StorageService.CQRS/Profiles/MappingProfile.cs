using AutoMapper;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.StorageService.CQRS.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Item, ItemDTO>().ReverseMap();
    }
}
