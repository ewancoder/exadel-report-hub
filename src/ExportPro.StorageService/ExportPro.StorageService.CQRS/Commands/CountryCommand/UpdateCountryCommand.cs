using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.CountryCommand;

public record UpdateCountryCommand(string Id, string Name, string? Code) : ICommand<bool>;
