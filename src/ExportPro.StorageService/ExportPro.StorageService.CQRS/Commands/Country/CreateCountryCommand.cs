using ExportPro.Common.Shared.Mediator;

namespace ExportPro.StorageService.CQRS.Commands.Country;

public class CreateCountryCommand : ICommand<Models.Models.Country>
{
    public string Name { get; set; } 
    public string? Code { get; set; }
}