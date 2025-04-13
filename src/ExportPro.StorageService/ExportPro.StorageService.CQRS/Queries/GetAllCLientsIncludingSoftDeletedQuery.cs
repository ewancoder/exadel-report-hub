using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Queries;

public record GetAllCLientsIncludingSoftDeletedQuery:IQuery<List<ClientResponse>>;
