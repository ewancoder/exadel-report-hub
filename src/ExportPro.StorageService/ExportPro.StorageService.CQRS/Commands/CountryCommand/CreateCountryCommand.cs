using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Commands.CountryCommand;

public class CreateCountryCommand : ICommand<Country>
{
    public string Name { get; set; }
    public string? Code { get; set; }
}