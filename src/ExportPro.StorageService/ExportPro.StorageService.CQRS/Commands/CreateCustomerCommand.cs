using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Commands;

public class CreateCustomerCommand : ICommand<Customer>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
}
