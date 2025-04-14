using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Queries.Client;

public record GetClientsQuery : IQuery<List<ClientResponse>>;
