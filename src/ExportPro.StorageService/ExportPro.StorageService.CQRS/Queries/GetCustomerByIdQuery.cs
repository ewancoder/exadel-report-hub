using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries;

public class GetCustomerByIdQuery : IQuery<Customer>
{
    public ObjectId Id { get; set; }
}