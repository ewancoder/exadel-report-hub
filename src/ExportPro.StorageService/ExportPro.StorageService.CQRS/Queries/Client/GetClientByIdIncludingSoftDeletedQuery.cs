using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.Client;

public record GetClientByIdIncludingSoftDeletedQuery(ObjectId Id) : IQuery<ClientResponse>;
