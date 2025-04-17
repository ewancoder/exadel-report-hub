using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Commands.CustomerCommand;

public class CreateCustomerCommand : ICommand<Customer>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string CountryId { get; set; }
}