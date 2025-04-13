using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Queries.Client;

public record GetAllCLientsIncludingSoftDeletedQuery : IQuery<List<ClientResponse>>;
