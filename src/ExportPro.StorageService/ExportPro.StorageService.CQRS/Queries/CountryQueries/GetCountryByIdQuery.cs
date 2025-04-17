using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.CountryQueries;

public record GetCountryByIdQuery(ObjectId Id) : IQuery<Country>;