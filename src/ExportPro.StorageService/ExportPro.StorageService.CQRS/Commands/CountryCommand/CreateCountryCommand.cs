using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;

namespace ExportPro.StorageService.CQRS.Commands.CountryCommand;

public class CreateCountryCommand : ICommand<CountryDto>
{
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? currencyId { get; set; }
}
