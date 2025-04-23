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
        PdfInvoiceExportDto invoice =
            await storageApi.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);

        // build PDF
        byte[] bytes = pdfGenerator.GeneratePdf(invoice);
        string fileName = $"invoice_{invoice.InvoiceNumber}.pdf";

        return new PdfFileDto(fileName, bytes);
    }
}