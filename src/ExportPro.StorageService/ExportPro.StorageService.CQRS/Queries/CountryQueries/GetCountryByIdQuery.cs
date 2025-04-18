using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;

namespace ExportPro.StorageService.CQRS.Queries.CountryQueries;

public record GetCountryByIdQuery(string Id) : IQuery<CountryDto>;