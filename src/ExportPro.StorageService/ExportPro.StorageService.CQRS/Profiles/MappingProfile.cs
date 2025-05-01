using AutoMapper;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Routing.Constraints;

namespace ExportPro.StorageService.CQRS.Profiles;

public sealed class MappingProfile : Profile
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
        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId.ToGuid()))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId.ToGuid()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(x => x.ClientId.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.Id.ToObjectId()))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(y => y.CurrencyId.ToObjectId()))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(y => y.CustomerId.ToObjectId()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(y => y.ClientId.ToObjectId()));
        // Customer -> CustomerDto
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
