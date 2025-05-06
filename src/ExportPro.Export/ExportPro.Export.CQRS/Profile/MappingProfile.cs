using ExportPro.Export.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.Export.CQRS.Profile;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        CreateMap<InvoiceDto, PdfInvoiceExportDto>()
            .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.InvoiceNumber))
            .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.CurrencyCode, opt => opt.Ignore()) // Set manually in handler
            .ForMember(dest => dest.PaymentStatus,
                opt => opt.MapFrom(src => src.PaymentStatus.HasValue ? src.PaymentStatus.ToString() : null))
            .ForMember(dest => dest.BankAccountNumber, opt => opt.MapFrom(src => src.BankAccountNumber))
            .ForMember(dest => dest.ClientName, opt => opt.Ignore()) // Set manually in handler
            .ForMember(dest => dest.CustomerName, opt => opt.Ignore()) // Set manually in handler
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<ItemDtoForClient, PdfItemExportDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(d => d.CurrencyCode, o => o.Ignore());

        CreateMap<(List<InvoiceDto> Invoices, List<ItemResponse> Items, List<PlansResponse> Plans,
                ReportFilterDto Filters, string ClientName), ReportContentDto>()
            .ForMember(dest => dest.Invoices, opt => opt.MapFrom(src => src.Invoices))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.Plans, opt => opt.MapFrom(src => src.Plans))
            .ForMember(dest => dest.Filters, opt => opt.MapFrom(src => src.Filters))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName));
    }
}