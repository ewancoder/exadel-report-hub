using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands;

public class UpdateCustomerCommand : ICommand<Customer>
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
}