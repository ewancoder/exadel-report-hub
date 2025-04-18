using AutoMapper;
using ExportPro.StorageService.CQRS.Commands.CustomerCommand;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Routing.Constraints;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Item, ItemDTO>().ReverseMap();
        CreateMap<Item, ItemResponse>().ForMember(dest => dest.Id, src => src.MapFrom(
           x => x.Id.ToString()))
            .ReverseMap().ForMember(dest => dest.Id, src => src.MapFrom(
           y => ObjectId.Parse(y.Id)));
        CreateMap<Client, ClientResponse>().ForMember
            (dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ForMember(dest=>dest.Items,src=>src.MapFrom(x=> x.Items))
            .ReverseMap()
            .ForMember(dest =>dest.Id,src=>src.MapFrom(y=>ObjectId.Parse(y.Id))).
            ForMember(dest=>dest.Items,src=>src.MapFrom(y=>y.Items));
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Invoice, InvoiceResponse>().ForMember(dest => dest.Id, src => src.MapFrom(
            x => x.Id.ToString())).ReverseMap().ForMember(dest => dest.Id, src => src.MapFrom(
            y => ObjectId.Parse(y.Id)));
        CreateMap<ItemDtoForClient, Item>().ReverseMap();

        // Customer -> CustomerDto
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId.ToString()));
    }
}
