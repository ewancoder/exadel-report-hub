using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Country;

public record UpdateCountryCommand(ObjectId Id, string Name, string? Code) : ICommand<bool>;
