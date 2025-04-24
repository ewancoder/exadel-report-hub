using AutoMapper;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Item, ItemDTO>().ReverseMap();
        CreateMap<Item, ItemResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => ObjectId.Parse(y.Id)));
        CreateMap<Client, ClientResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => ObjectId.Parse(y.Id)));
        CreateMap<ItemDtoForClient, Item>().ReverseMap();
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => ObjectId.Parse(y.Id)));

        // Customer -> CustomerDto
        CreateMap<Customer, SDK.DTOs.CustomerDTOs.CustomerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember<string>(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId.ToString()));

        CreateMap<Country, CountryDto>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<CountryDto, Country>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)));
        CreateMap<ItemDtoForClient, ItemResponse>().ReverseMap();
        CreateMap<Plans, PlansResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => ObjectId.Parse(x.Id)));
        CreateMap<Plans, PlansDto>().ReverseMap();
        CreateMap<PlansResponse, PlansDto>().ReverseMap();
        CreateMap<Currency, CurrencyDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)));
        CreateMap<Customer, SDK.Responses.CustomerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)));
    }
}
