using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.Invoice;

public class GetInvoiceByIdQuery : IQuery<Models.Models.Invoice>
{
    public ObjectId Id { get; set; }
}
