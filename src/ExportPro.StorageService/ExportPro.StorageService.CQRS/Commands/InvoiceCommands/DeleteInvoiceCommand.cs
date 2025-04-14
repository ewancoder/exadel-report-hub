using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.InvoiceCommands;

public class DeleteInvoiceCommand : ICommand<bool>
{
    public ObjectId Id { get; set; }
}