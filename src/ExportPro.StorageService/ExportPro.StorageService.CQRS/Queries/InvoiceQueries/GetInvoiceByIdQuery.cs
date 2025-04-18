using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

namespace ExportPro.StorageService.CQRS.Queries.InvoiceQueries;

public class GetInvoiceByIdQuery : IQuery<InvoiceDto>
{
    public string Id { get; set; }
}