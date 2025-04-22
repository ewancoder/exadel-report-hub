using ExportPro.Export.SDK.DTOs;
using MediatR;

namespace ExportPro.Export.CQRS.Queries;

public record GenerateInvoicePdfQuery(string InvoiceId) : IRequest<InvoicePdfDto>;

public class GenerateInvoicePdfQueryHandler()
        : IRequestHandler<GenerateInvoicePdfQuery, InvoicePdfDto>
{
    public async Task<InvoicePdfDto> Handle(GenerateInvoicePdfQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}