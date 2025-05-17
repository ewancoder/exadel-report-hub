using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Export.Job.Utilities.Helpers;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Profiles;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Invoice, CreateInvoiceDto>()
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId.ToGuid()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(x => x.ClientId.ToGuid()))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ItemsId!.Select(y => y.ToGuid())))
            .ReverseMap()
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(y => y.CustomerId.ToObjectId()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(y => y.ClientId.ToObjectId()))
            .ForMember(dest => dest.CurrencyId, opt => opt.Ignore())
            .ForMember(dest => dest.ItemsId, opt => opt.MapFrom(src => src.Items!.Select(y => y.ToObjectId())));
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId.ToGuid()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(x => x.ClientId.ToGuid()))
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(y => y.CustomerId.ToObjectId()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(y => y.ClientId.ToObjectId()))
            .ForMember(dest => dest.CurrencyId, opt => opt.Ignore())
            .ForMember(dest => dest.ItemsId, opt => opt.Ignore());
        CreateMap<Item, ItemDTO>().ReverseMap();
        CreateMap<Item, ItemResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.Id.ToObjectId()));
        CreateMap<Client, ClientResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.Id.ToObjectId()));
        CreateMap<ItemDtoForClient, Item>()
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId.ToObjectId()))
            .ReverseMap()
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId.ToGuid()));
        CreateMap<ItemDtoForInvoice, Item>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToObjectId()))
            .ForMember(dest => dest.CurrencyId, src => src.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ForMember(dest => dest.Currency, src => src.Ignore());

        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id.ToGuid()))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(x => x.CustomerId.ToGuid()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(x => x.ClientId.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.MapFrom(y => y.Id.ToObjectId()))
            .ForMember(dest => dest.CustomerId, src => src.MapFrom(y => y.CustomerId.ToObjectId()))
            .ForMember(dest => dest.ClientId, src => src.MapFrom(y => y.ClientId.ToObjectId()));

        CreateMap<CreateInvoiceDto, InvoiceResponse>().ReverseMap();
        CreateMap<CreateInvoiceDto, InvoiceDto>().ReverseMap();

        // Customer -> CustomerDto
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
            .ForMember(dest => dest.CountryId, src => src.MapFrom(x => x.CountryId.ToGuid()));
        CreateMap<Country, CountryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
            .ForMember(dest => dest.CurrencyId, src => src.MapFrom(x => x.CurrencyId.ToGuid()));
        CreateMap<Customer, CreateUpdateCustomerDto>()
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId.ToGuid()))
            .ReverseMap()
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId.ToObjectId()));
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

        CreateMap<ReportPreference, ReportPreferenceResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToGuid()))
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId.ToGuid()))
            .ForMember(
                dest => dest.HumanReadableSchedule,
                opt => opt.MapFrom(src => CronToTextHelper.ToReadableText(src.CronExpression))
            )
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToObjectId()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToObjectId()))
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId.ToObjectId()));
    }
}
