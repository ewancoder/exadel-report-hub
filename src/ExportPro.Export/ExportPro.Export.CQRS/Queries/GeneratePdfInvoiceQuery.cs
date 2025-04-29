using System.Security.Claims;
using AutoMapper;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExportPro.Export.CQRS.Queries;

public record GenerateInvoicePdfQuery(string InvoiceId) : IRequest<PdfFileDto>;

public sealed class GenerateInvoicePdfQueryHandler(
    IStorageServiceApi storageApi,
    IPdfGenerator pdfGenerator,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GenerateInvoicePdfQueryHandler> logger)
    : IRequestHandler<GenerateInvoicePdfQuery, PdfFileDto>
{
    public async Task<PdfFileDto> Handle(GenerateInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        ValidateInvoiceId(request.InvoiceId);

        var userId = httpContextAccessor.HttpContext?.User?
                         .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        logger.LogInformation("GenerateInvoicePdf START: user {UserId}, invoice {InvoiceId}, ts {Ts}",
                              userId, request.InvoiceId, DateTime.UtcNow);

        try
        {
            var invoiceDto = await GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
            string currency = await GetCurrencyCodeAsync(invoiceDto.CurrencyId, cancellationToken);
            string client = await GetClientNameAsync(invoiceDto.ClientId, cancellationToken);
            string customer = await GetCustomerNameAsync(invoiceDto.CustomerId, cancellationToken);
            var invoice = MapToPdfInvoiceExportDto(invoiceDto, currency, client, customer);
            var result = GeneratePdfFile(invoice);

            logger.LogInformation("GenerateInvoicePdf DONE: user {UserId}, invoice {InvoiceId}, ts {Ts}",
                                  userId, request.InvoiceId, DateTime.UtcNow);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "GenerateInvoicePdf FAILED: user {UserId}, invoice {InvoiceId}, ts {Ts}",
                userId, request.InvoiceId, DateTime.UtcNow);
            throw;
        }
    }

    private static void ValidateInvoiceId(string invoiceId)
    {
        if (string.IsNullOrWhiteSpace(invoiceId))
            throw new ArgumentException("InvoiceId is required.", nameof(invoiceId));
    }

    private async Task<InvoiceDto> GetInvoiceByIdAsync(string id, CancellationToken ct)
    {
        var resp = await storageApi.GetInvoiceByIdAsync(id, ct);
        return resp.Data ?? throw new InvalidOperationException("Storage-service returned no data");
    }

    private async Task<string> GetCurrencyCodeAsync(string? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id))
            return "—";

        var resp = await storageApi.GetCurrencyByIdAsync(id, ct);
        return resp.Data?.CurrencyCode ?? "—";
    }

    private async Task<string> GetClientNameAsync(string? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id))
            return "—";

        var resp = await storageApi.GetClientByIdAsync(id, ct);
        return resp.Data?.Name ?? "—"; // fixed for BaseResponse<ClientResponse>
    }

    private async Task<string> GetCustomerNameAsync(string? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id))
            return "—";

        var resp = await storageApi.GetCustomerByIdAsync(id, ct);
        return resp.Data?.Name ?? "—";
    }

    private PdfInvoiceExportDto MapToPdfInvoiceExportDto(
        InvoiceDto src, string currency, string client, string customer)
    {
        var dest = mapper.Map<PdfInvoiceExportDto>(src);
        dest.CurrencyCode = currency;
        dest.ClientName = client;
        dest.CustomerName = customer;
        return dest;
    }

    private PdfFileDto GeneratePdfFile(PdfInvoiceExportDto invoice)
    {
        byte[] bytes = pdfGenerator.GeneratePdf(invoice);
        string name = FileNameTemplates.InvoicePdfFileName(invoice.InvoiceNumber);
        return new PdfFileDto(name, bytes);
    }
}
