using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.CustomerQueries;
public class GetCustomerByIdQuery : IQuery<CustomerDto>
{
    public ObjectId Id { get; set; }
}