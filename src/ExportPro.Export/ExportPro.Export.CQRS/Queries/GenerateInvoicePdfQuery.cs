using AutoMapper;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Export.CQRS.Queries;

public record GenerateInvoicePdfQuery([FromRoute] string InvoiceId) : IRequest<PdfFileDto>;

public sealed class GenerateInvoicePdfQueryHandler(
    IStorageServiceApi storageApi,
    IPdfGenerator pdfGenerator,
    IMapper mapper)
    : IRequestHandler<GenerateInvoicePdfQuery, PdfFileDto>
{
    public async Task<PdfFileDto> Handle(GenerateInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        ValidateInvoiceId(request.InvoiceId);
        var invoiceDto = await GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
        string currencyCode = await GetCurrencyCodeAsync(invoiceDto.CurrencyId, cancellationToken);
        string clientName = await GetClientNameAsync(invoiceDto.ClientId, cancellationToken);
        string customerName = await GetCustomerNameAsync(invoiceDto.CustomerId, cancellationToken);
        var invoice = MapToPdfInvoiceExportDto(invoiceDto, currencyCode, clientName, customerName);
        return GeneratePdfFile(invoice);
    }

    private static void ValidateInvoiceId(string invoiceId)
    {
        if (string.IsNullOrWhiteSpace(invoiceId))
            throw new ArgumentException("InvoiceId is required.", nameof(invoiceId));
    }

    private async Task<InvoiceDto> GetInvoiceByIdAsync(string invoiceId, CancellationToken cancellationToken)
    {
        var apiResponse = await storageApi.GetInvoiceByIdAsync(invoiceId, cancellationToken);
        return apiResponse.Data ?? throw new InvalidOperationException("Storage-service returned no data");
    }

    private async Task<string> GetCurrencyCodeAsync(string? currencyId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currencyId))
            return "—";

        var curResp = await storageApi.GetCurrencyByIdAsync(currencyId, cancellationToken);
        return curResp.Data?.CurrencyCode ?? "—";
    }

    private async Task<string> GetClientNameAsync(string? clientId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return "—";

        var clientResp = await storageApi.GetClientByIdAsync(clientId, cancellationToken);
        return clientResp.Data?.TModel?.Name ?? "—";
    }

    private async Task<string> GetCustomerNameAsync(string? customerId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return "—";

        var custResp = await storageApi.GetCustomerByIdAsync(customerId, cancellationToken);
        return custResp.Data?.Name ?? "—";
    }

    private PdfInvoiceExportDto MapToPdfInvoiceExportDto(InvoiceDto invoiceDto, string currencyCode, string clientName, string customerName)
    {
        var invoice = mapper.Map<PdfInvoiceExportDto>(invoiceDto);
        invoice.CurrencyCode = currencyCode;
        invoice.ClientName = clientName;
        invoice.CustomerName = customerName;
        return invoice;
    }

    private PdfFileDto GeneratePdfFile(PdfInvoiceExportDto invoice)
    {
        byte[] bytes = pdfGenerator.GeneratePdf(invoice);
        string fileName = $"invoice_{invoice.InvoiceNumber}.pdf";
        return new PdfFileDto(fileName, bytes);
    }
}

