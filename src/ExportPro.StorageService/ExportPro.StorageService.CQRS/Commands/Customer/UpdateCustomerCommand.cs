using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Customer;

public class UpdateCustomerCommand : ICommand<Models.Models.Customer>
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
}