using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Commands.CustomerCommand;

public class UpdateCustomerCommand : ICommand<Customer>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string CountryId { get; set; }
}