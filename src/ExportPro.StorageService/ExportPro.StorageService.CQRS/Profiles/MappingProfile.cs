using AutoMapper;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
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
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name))
            .ForMember(dest => dest.Description, src => src.MapFrom(x => x.Description))
            .ForMember(dest => dest.Price, src => src.MapFrom(x => x.Price))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId))
            .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => ObjectId.Parse(y.Id)))
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name))
            .ForMember(dest => dest.Description, src => src.MapFrom(x => x.Description))
            .ForMember(dest => dest.Price, src => src.MapFrom(x => x.Price))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId))
            .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status));

        CreateMap<Client, ClientResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => ObjectId.Parse(y.Id)));
        CreateMap<ItemDtoForClient, Item>().ReverseMap();
        CreateMap<Client, ClientDto>().ReverseMap();

        CreateMap<ItemDtoForClient, ItemResponse>().ReverseMap();
        CreateMap<Plans, PlansResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => ObjectId.Parse(x.Id)));
        CreateMap<Plans, PlansDto>().ReverseMap();
        CreateMap<PlansResponse, PlansDto>().ReverseMap();

        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToString()))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(x => x.ClientId))
            .ForMember(dest => dest.IssueDate, src => src.MapFrom(x => x.IssueDate))
            .ForMember(dest => dest.Amount, src => src.MapFrom(x => x.Amount))
            .ForMember(dest => dest.PaymentStatus, src => src.MapFrom(x => x.PaymentStatus))
            .ForMember(dest => dest.DueDate, src => src.MapFrom(x => x.DueDate))
            .ForMember(dest => dest.BankAccountNumber, src => src.MapFrom(x => x.BankAccountNumber))
            .ForMember(dest => dest.Items, src => src.MapFrom(x => x.Items))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => ObjectId.Parse(y.Id)))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(x => x.ClientId))
            .ForMember(dest => dest.IssueDate, src => src.MapFrom(x => x.IssueDate))
            .ForMember(dest => dest.Amount, src => src.MapFrom(x => x.Amount))
            .ForMember(dest => dest.PaymentStatus, src => src.MapFrom(x => x.PaymentStatus))
            .ForMember(dest => dest.DueDate, src => src.MapFrom(x => x.DueDate))
            .ForMember(dest => dest.Items, src => src.MapFrom(x => x.Items))
            .ForMember(dest => dest.BankAccountNumber, src => src.MapFrom(x => x.BankAccountNumber));

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
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId.ToString()));

        CreateMap<Currency, CurrencyResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.CurrencyCode))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.CurrencyCode));

        CreateMap<Country, CountryDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
           .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId))
           .ReverseMap()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)))
           .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId));
    }
}