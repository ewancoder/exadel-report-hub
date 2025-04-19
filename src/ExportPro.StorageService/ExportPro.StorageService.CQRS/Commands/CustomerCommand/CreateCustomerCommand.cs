using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Commands.CustomerCommand;

public class CreateCustomerCommand : ICommand<CustomerResponse>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string CountryId { get; set; }
}
