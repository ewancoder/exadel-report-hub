using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.Country;

public record GetCountryByIdQuery(ObjectId Id) : IQuery<Models.Models.Country>;