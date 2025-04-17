﻿using AutoMapper;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Routing.Constraints;
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
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => ObjectId.Parse(y.Id)));
        CreateMap<ItemDtoForClient, Item>().ReverseMap();
        CreateMap<ItemDtoForClient, ItemResponse>().ReverseMap();
        CreateMap<Plans, PlansResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => ObjectId.Parse(x.Id)));
        CreateMap<Plans, PlansDto>().ReverseMap();
        CreateMap<PlansResponse, PlansDto>().ReverseMap();
    }
}
