using ExportPro.StorageService.CQRS.Profiles.ItemMaps;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Profiles.InvoiceMaps;

public static class InvoiceMapper
{
    public static InvoiceResponse ToDto(Invoice invoice)
    {
        return new InvoiceResponse
        {
            Id = invoice.Id.ToString(),
            InvoiceNumber = invoice.InvoiceNumber ?? string.Empty,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Amount = (decimal)(invoice.Amount ?? 0),
            CurrencyId = invoice.CurrencyId ?? string.Empty,
            PaymentStatus = invoice.PaymentStatus,
            CustomerId = invoice.CustomerId ?? string.Empty,
            BankAccountNumber = invoice.BankAccountNumber ?? string.Empty,
            ClientId = invoice.ClientId ?? string.Empty,
            Items = invoice.Items?.Select(ItemMapper.ToDto).ToList() ?? []
        };
    }
}