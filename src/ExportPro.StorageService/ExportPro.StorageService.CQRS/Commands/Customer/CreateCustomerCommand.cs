using ExportPro.Common.Shared.Mediator;

namespace ExportPro.StorageService.CQRS.Commands.Customer;

public class CreateCustomerCommand : ICommand<Models.Models.Customer>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string CountryId { get; set; }
}