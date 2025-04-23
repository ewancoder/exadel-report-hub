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
        var apiResp = await storageApi.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);

        var src = apiResp.Data ?? throw new InvalidOperationException("Storage-service returned no data");

        string currencyCode = "—";

        if (!string.IsNullOrWhiteSpace(src.CurrencyId))
        {
            var curResp = await storageApi.GetCurrencyByIdAsync(src.CurrencyId, cancellationToken);
            currencyCode = curResp.Data?.CurrencyCode ?? "—";
        }

        string clientName = "—";

        if (!string.IsNullOrWhiteSpace(src.ClientId))
        {
            var clientResp = await storageApi.GetClientByIdAsync(src.ClientId, cancellationToken);
            clientName = clientResp.Data?.TModel?.Name ?? "—";
        }

        string customerName = "—";

        if (!string.IsNullOrWhiteSpace(src.CustomerId))
        {
            var custResp = await storageApi.GetCustomerByIdAsync(src.CustomerId, cancellationToken);
            customerName = custResp.Data?.Name ?? "—";
        }

        PdfInvoiceExportDto invoice = new()
        {
            Id = src.Id,
            InvoiceNumber = src.InvoiceNumber,
            IssueDate = src.IssueDate,
            DueDate = src.DueDate,
            Amount = src.Amount,
            CurrencyCode = currencyCode,
            PaymentStatus = src.PaymentStatus?.ToString(),
            BankAccountNumber = src.BankAccountNumber,
            ClientName = clientName,
            CustomerName = customerName,
            Items = src.Items?.Select(i => new PdfItemExportDto
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