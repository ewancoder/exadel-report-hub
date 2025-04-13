using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Customer;

public class DeleteCustomerCommand : ICommand<bool>
{
    public ObjectId Id { get; set; }
}