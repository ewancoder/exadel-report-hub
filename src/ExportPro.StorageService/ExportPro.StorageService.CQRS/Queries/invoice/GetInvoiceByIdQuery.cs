using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.invoice;

public class GetInvoiceByIdQuery : IQuery<Invoice>
{
    public ObjectId Id { get; set; }
}
