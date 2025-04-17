using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Queries.CountryQueries;

public record GetAllCountriesQuery : IQuery<List<Country>>;