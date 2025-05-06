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

public record GenerateInvoicePdfQuery(Guid InvoiceId) : IRequest<PdfFileDto>;

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
        if (request.InvoiceId == Guid.Empty)
            throw new ArgumentException("InvoiceId cannot be empty", nameof(request.InvoiceId));

        var userId = httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        logger.LogInformation("GenerateInvoicePdf START: user {UserId}, invoice {InvoiceId}, ts {Ts}",
            userId, request.InvoiceId, DateTime.UtcNow);

        var invoiceDto = await GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
        string currency = await GetCurrencyCodeAsync(invoiceDto.CurrencyId, cancellationToken);
        string client = await GetClientNameAsync(invoiceDto.ClientId, cancellationToken);
        string customer = await GetCustomerNameAsync(invoiceDto.CustomerId, cancellationToken);
        var invoice = MapToPdfInvoiceExportDto(invoiceDto, currency, client, customer);
        await PopulateItemCurrencyCodesAsync(invoiceDto, invoice, cancellationToken);
        var result = GeneratePdfFile(invoice);

        logger.LogInformation("GenerateInvoicePdf DONE: user {UserId}, invoice {InvoiceId}, ts {Ts}",
            userId, request.InvoiceId, DateTime.UtcNow);

        return result;
    }

    private async Task PopulateItemCurrencyCodesAsync(
        InvoiceDto srcInvoice,
        PdfInvoiceExportDto destInvoice,
        CancellationToken ct)
    {
        if (srcInvoice.Items is null)
            return;

        var cache = new Dictionary<Guid, string>();

        for (int i = 0; i < srcInvoice.Items.Count; i++)
        {
            var curId = srcInvoice.Items[i].CurrencyId;
            
            if (curId == Guid.Empty)
            {
                destInvoice.Items[i].CurrencyCode = "—";
                continue;
            }

            if (!cache.TryGetValue(curId, out var code))
            {
                code = await GetCurrencyCodeAsync(curId, ct);
                cache[curId] = code;
            }

            destInvoice.Items[i].CurrencyCode = code;
        }
    }

    private async Task<InvoiceDto> GetInvoiceByIdAsync(Guid id, CancellationToken ct)
    {
        var resp = await storageApi.GetInvoiceByIdAsync(id, ct);
        return resp.Data ?? throw new InvalidOperationException("Storage-service returned no data");
    }

    private async Task<string> GetCurrencyCodeAsync(Guid? id, CancellationToken ct)
    {
        if (id is null || id == Guid.Empty)
            return "—";

        var resp = await storageApi.GetCurrencyByIdAsync(id.Value, ct);
        return resp.Data?.CurrencyCode ?? "—";
    }

    private async Task<string> GetClientNameAsync(Guid? id, CancellationToken ct)
    {
        if (id is null || id == Guid.Empty)
            return "—";

        var resp = await storageApi.GetClientByIdAsync(id.Value, ct);
        return resp.Data?.Name ?? "—";
    }

    private async Task<string> GetCustomerNameAsync(Guid? id, CancellationToken ct)
    {
        if (id is null || id == Guid.Empty)
            return "—";

        var resp = await storageApi.GetCustomerByIdAsync(id.Value, ct);
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
        byte[] bytes = pdfGenerator.GeneratePdfDocument(invoice);
        string name = FileNameTemplates.InvoicePdfFileName(invoice.InvoiceNumber);
        return new PdfFileDto(name, bytes);
    }
}