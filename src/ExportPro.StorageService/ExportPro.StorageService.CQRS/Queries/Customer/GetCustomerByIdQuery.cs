using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.Customer;

public class GetCustomerByIdQuery : IQuery<Models.Models.Customer>
{
    public ObjectId Id { get; set; }
}