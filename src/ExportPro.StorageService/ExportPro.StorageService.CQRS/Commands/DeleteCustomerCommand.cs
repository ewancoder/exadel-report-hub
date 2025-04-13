using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands;

public class DeleteCustomerCommand : ICommand<bool>
{
    public ObjectId Id { get; set; }
}