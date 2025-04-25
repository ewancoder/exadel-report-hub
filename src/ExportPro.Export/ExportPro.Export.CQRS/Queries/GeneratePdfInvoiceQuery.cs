using System.Security.Claims;
using DnsClient.Internal;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExportPro.Export.CQRS.Queries;

public record GeneratePdfInvoiceQuery(string InvoiceId) : IRequest<PdfFileDto>;

public sealed class GenerateInvoicePdfQueryHandler(
    IStorageServiceApi storageApi, 
    IPdfGenerator pdfGenerator,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GeneratePdfInvoiceQuery> logger)
        : IRequestHandler<GeneratePdfInvoiceQuery, PdfFileDto>
{
    public async Task<PdfFileDto> Handle(GeneratePdfInvoiceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.InvoiceId))
            throw new ArgumentException("InvoiceId is required.", nameof(request.InvoiceId));

        var apiResponce = await storageApi.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
        var invoiceDto = apiResponce.Data ?? throw new InvalidOperationException("Storage-service returned no data");
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        logger.LogInformation(
            "Start GeneratePdfInvoiceQuery: User {@UserId}, " +
            "InvoiceId {@InvoiceId}," +
            " Timestamp {@Timestamp}",
            userId, request.InvoiceId, DateTime.UtcNow);
        string currencyCode = "—";
        try
        {
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
        // TODO: should i use instead of it AutoMapper?
        PdfInvoiceExportDto invoice = new()
        {
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
                Price = i.Price
            }).ToList() ?? []
        };

        // build PDF
        byte[] bytes = pdfGenerator.GeneratePdf(invoice);
        string fileName = $"invoice_{invoice.InvoiceNumber}.pdf";
        logger.LogInformation(
                   "Finished GeneratePdfInvoiceQuery: User {@UserId}, InvoiceId {@InvoiceId}, " +
                   "Timestamp {@Timestamp} Status : Success",
                   userId, request.InvoiceId, DateTime.UtcNow);
            return new PdfFileDto(fileName, bytes);
        }
        catch(Exception ex)
        {
            logger.LogError(
               ex,
               "Error in GeneratePdfInvoiceQuery: User {UserId}, InvoiceId {InvoiceId}, Timestamp {Timestamp}, Status : Failed",
               userId, request.InvoiceId, DateTime.UtcNow);
            throw;
        }
    }
}
