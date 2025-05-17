using System.Security.Claims;
using AutoMapper;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Refit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExportPro.Export.CQRS.Queries;

public record GenerateInvoicePdfQuery(Guid InvoiceId) : IRequest<PdfFileDto>;

public sealed class GenerateInvoicePdfQueryHandler(
    IStorageServiceApi storageServiceApi,
    IPdfGenerator pdfGenerator,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GenerateInvoicePdfQueryHandler> logger
) : IRequestHandler<GenerateInvoicePdfQuery, PdfFileDto>
{
    public async Task<PdfFileDto> Handle(GenerateInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        if (request.InvoiceId == Guid.Empty)
            throw new ArgumentException("InvoiceId cannot be empty", nameof(request.InvoiceId));

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        logger.LogInformation(
            "GenerateInvoicePdf START: user {UserId}, invoice {InvoiceId}, ts {Ts}",
            userId,
            request.InvoiceId,
            DateTime.UtcNow
        );

        var result = await CreateInvoicePdfAsync(request, cancellationToken);

        logger.LogInformation(
            "GenerateInvoicePdf DONE: user {UserId}, invoice {InvoiceId}, ts {Ts}",
            userId,
            request.InvoiceId,
            DateTime.UtcNow
        );

        return result;
    }

    private async Task<PdfFileDto> CreateInvoicePdfAsync(
        GenerateInvoicePdfQuery request,
        CancellationToken cancellationToken
    )
    {
        var invoiceDto = await GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
        var clientTask = GetClientNameAsync(invoiceDto.ClientId, cancellationToken);
        var customerTask = GetCustomerNameAsync(invoiceDto.CustomerId, cancellationToken);
        await Task.WhenAll(clientTask, customerTask);
        var client = await clientTask;
        var customer = await customerTask;
        var items = invoiceDto.Items;
        var invoice = MapToPdfInvoiceExportDto(invoiceDto, client, customer, invoiceDto.Currency, items);
        return GeneratePdfFileDto(invoice);
    }

    private async Task<InvoiceDto> GetInvoiceByIdAsync(Guid id, CancellationToken ct)
    {
        var resp = await storageServiceApi.Invoice.GetById(id, ct);
        return resp.Data ?? throw new InvalidOperationException("Storage-service returned no data");
    }

    private async Task<string> GetClientNameAsync(Guid? id, CancellationToken ct)
    {
        if (id is null || id == Guid.Empty)
            return "—";

        var resp = await storageServiceApi.Client.GetClientById(id.Value, ct);
        return resp.Data?.Name ?? "—";
    }

    private async Task<string> GetCustomerNameAsync(Guid? id, CancellationToken ct)
    {
        if (id is null || id == Guid.Empty)
            return "—";

        var resp = await storageServiceApi.Customer.GetById(id.Value, ct);
        return resp.Data?.Name ?? "—";
    }

    private PdfInvoiceExportDto MapToPdfInvoiceExportDto(
        InvoiceDto src,
        string client,
        string customer,
        string currencyCode,
        List<ItemDtoForInvoice> items
    )
    {
        var dest = mapper.Map<PdfInvoiceExportDto>(src);
        dest.ClientName = client;
        dest.CurrencyCode = currencyCode;
        dest.CustomerName = customer;
        dest.Items = items
            .Select(i => new PdfItemExportDto()
            {
                Name = i.Name,
                Price = i.Price,
                CurrencyCode = i.Currency,
            })
            .ToList();

        return dest;
    }

    private PdfFileDto GeneratePdfFileDto(PdfInvoiceExportDto invoice)
    {
        var bytes = pdfGenerator.GeneratePdfDocument(invoice);
        var name = FileNameTemplates.InvoicePdfFileName(invoice.InvoiceNumber!);
        return new PdfFileDto(name, bytes);
    }
}
