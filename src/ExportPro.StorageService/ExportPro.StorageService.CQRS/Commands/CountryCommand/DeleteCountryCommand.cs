using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.CountryCommand;
public record DeleteCountryCommand(string Id) : ICommand<bool>;