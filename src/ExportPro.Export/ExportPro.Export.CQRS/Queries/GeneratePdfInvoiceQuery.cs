using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using MediatR;

namespace ExportPro.Export.CQRS.Queries;

public record GeneratePdfInvoiceQuery(string InvoiceId) : IRequest<PdfFileDto>;

public sealed class GenerateInvoicePdfQueryHandler(
    IStorageServiceApi storageApi,
    IPdfGenerator pdfGenerator)
        : IRequestHandler<GeneratePdfInvoiceQuery, PdfFileDto>
{
    public async Task<PdfFileDto> Handle(
        GeneratePdfInvoiceQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.InvoiceId))
            throw new ArgumentException("InvoiceId is required.", nameof(request.InvoiceId));

        // fetch plain DTO
        var apiResponce = await storageApi.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
        var invoiceDto = apiResponce.Data ?? throw new InvalidOperationException("Storage-service returned no data");

        string currencyCode = "—";
        if (!string.IsNullOrWhiteSpace(invoiceDto.CurrencyId))
        {
            var curResp = await storageApi.GetCurrencyByIdAsync(invoiceDto.CurrencyId, cancellationToken);
            currencyCode = curResp.Data?.CurrencyCode ?? "—";
        }

        string clientName = "—";
        if (!string.IsNullOrWhiteSpace(invoiceDto.ClientId))
        {
            var clientResp = await storageApi.GetClientByIdAsync(invoiceDto.ClientId, cancellationToken);
            clientName = clientResp.Data?.TModel?.Name ?? "—";
        }

        string customerName = "—";
        if (!string.IsNullOrWhiteSpace(invoiceDto.CustomerId))
        {
            var custResp = await storageApi.GetCustomerByIdAsync(invoiceDto.CustomerId, cancellationToken);
            customerName = custResp.Data?.Name ?? "—";
        }

        // TODO: instead use AutoMapper
        PdfInvoiceExportDto invoice = new()
        {
            Id = invoiceDto.Id,
            InvoiceNumber = invoiceDto.InvoiceNumber,
            IssueDate = invoiceDto.IssueDate,
            DueDate = invoiceDto.DueDate,
            Amount = invoiceDto.Amount,
            CurrencyCode = currencyCode,
            PaymentStatus = invoiceDto.PaymentStatus?.ToString(),
            BankAccountNumber = invoiceDto.BankAccountNumber,
            ClientName = clientName,
            CustomerName = customerName,
            Items = invoiceDto.Items?.Select(i => new PdfItemExportDto
            {
                Name = i.Name,
                Price = (decimal)i.Price
            }).ToList() ?? []
        };

        // build PDF
        byte[] bytes = pdfGenerator.GeneratePdf(invoice);
        string fileName = $"invoice_{invoice.InvoiceNumber}.pdf";

        return new PdfFileDto(fileName, bytes);
    }
}