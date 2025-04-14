using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Client;

public record DeleteClientCommand(ObjectId ClientId) : ICommand<string>;