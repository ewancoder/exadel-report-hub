using AutoMapper;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Routing.Constraints;

namespace ExportPro.StorageService.CQRS.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Item, ItemDTO>().ReverseMap();
        CreateMap<Item, ItemResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.Id.ToObjectId()))
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.CurrencyId.ToObjectId()));
        CreateMap<Client, ClientResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.Id.ToObjectId()));
        CreateMap<ItemDtoForClient, Item>()
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId.ToObjectId()))
            .ReverseMap()
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId.ToGuid()));
        CreateMap<Client, ClientDto>().ReverseMap();

        CreateMap<ItemDtoForClient, ItemResponse>().ReverseMap();
        CreateMap<Plans, PlansResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => ObjectId.Parse(x.Id)));
        CreateMap<Plans, PlansDto>().ReverseMap();
        CreateMap<PlansResponse, PlansDto>().ReverseMap();

        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.Id.ToObjectId()));

        CreateMap<Customer, CustomerResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId));

        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
            .ForMember(dest => dest.CountryId, src => src.MapFrom(x => x.CountryId.ToGuid()));
        CreateMap<Country, CountryDto>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()));

        CreateMap<CountryDto, Country>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToObjectId()));
        CreateMap<ItemDtoForClient, ItemResponse>().ReverseMap();
        CreateMap<Plans, PlansResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToObjectId()));
        CreateMap<Plans, PlansDto>().ReverseMap();
        CreateMap<PlansResponse, PlansDto>().ReverseMap();
        CreateMap<Currency, CurrencyResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToObjectId()));
        CreateMap<Customer, CustomerResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
            .ForMember(dest => dest.CountryId, src => src.MapFrom(x => x.CountryId.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToObjectId()))
            .ForMember(dest => dest.CountryId, src => src.MapFrom(x => x.CountryId.ToObjectId()));
    }
}