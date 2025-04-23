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

        // light, in-place mapper – keeps export service completely decoupled
        PdfInvoiceExportDto invoice = new()
        {
            Id = src.Id,
            InvoiceNumber = src.InvoiceNumber,
            IssueDate = src.IssueDate,
            DueDate = src.DueDate,
            Amount = src.Amount,
            CurrencyId = src.CurrencyId,
            PaymentStatus = src.PaymentStatus?.ToString(),
            BankAccountNumber = src.BankAccountNumber,
            ClientId = src.ClientId,
            CustomerId = src.CustomerId,
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