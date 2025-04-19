using ExportPro.Common.Shared.Mediator;

namespace ExportPro.StorageService.CQRS.Commands.CountryCommand;
public record DeleteCountryCommand(string Id) : ICommand<bool>;