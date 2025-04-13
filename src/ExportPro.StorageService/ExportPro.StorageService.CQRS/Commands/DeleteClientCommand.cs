using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands;

public record DeleteClientCommand(ObjectId ClientId):ICommand<string>;