using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;

public sealed record GetClientsQuery(PaginationParameters PaginationParameters) : IQuery<PaginatedList<ClientResponse>>;

public sealed class GetClientsQueryHandler(IClientRepository clientRepository, IMapper mapper)
    : IQueryHandler<GetClientsQuery, PaginatedList<ClientResponse>>
{
    public async Task<BaseResponse<PaginatedList<ClientResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken
    )
    {
        var clients = await clientRepository.GetClients(request.PaginationParameters, cancellationToken);
        return new SuccessResponse<PaginatedList<ClientResponse>>(clients, "Clients Retrieved");
    }
}
