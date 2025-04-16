using ExportPro.Common.Shared.Mediator;

namespace ExportPro.StorageService.CQRS.Queries.Country;

public record GetAllCountriesQuery : IQuery<List<Models.Models.Country>>;