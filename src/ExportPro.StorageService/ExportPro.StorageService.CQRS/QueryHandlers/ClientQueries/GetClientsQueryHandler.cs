using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Refit;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;

public sealed record GetClientsQuery(PaginationParameters PaginationParameters)
    : IQuery<PaginatedList<ClientResponse>>,
        IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = null;
    public Resource Resource { get; init; } = Resource.Clients;
    public CrudAction Action { get; init; } = CrudAction.Read;
}

public sealed class GetClientsQueryHandler(
    IClientRepository clientRepository,
    IACLSharedApi aclApi,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper
) : IQueryHandler<GetClientsQuery, PaginatedList<ClientResponse>>
{
    public async Task<BaseResponse<PaginatedList<ClientResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userRole = TokenHelper.GetUserRole(httpContextAccessor.HttpContext.User);
        if (userRole == Role.SuperAdmin)
        {
            var allClients = await clientRepository.GetClients(request.PaginationParameters, cancellationToken);
            return new SuccessResponse<PaginatedList<ClientResponse>>(
                allClients
                    .Items.Select(x => mapper.Map<ClientResponse>(x))
                    .ToList()
                    .ToPaginatedList(request.PaginationParameters.PageNumber, request.PaginationParameters.PageSize)
            );
        }

        var availableClients = await aclApi.GetUserClientsAsync(cancellationToken);
        var clientIds = availableClients?.Data?.Select(id => id.ToObjectId()).ToList();
        if (clientIds == null)
            return new BadRequestResponse<PaginatedList<ClientResponse>>("Clients not found");
        var clients = await clientRepository.GetClientsByIdsAsync(
            clientIds,
            request.PaginationParameters,
            cancellationToken
        );
        if (clients.Items.Count == 0)
            return new BadRequestResponse<PaginatedList<ClientResponse>>("There is no such document");
        var clientsResponse = clients.Items.Select(x => mapper.Map<ClientResponse>(x)).ToList();
        return new SuccessResponse<PaginatedList<ClientResponse>>(
            clientsResponse.ToPaginatedList(
                request.PaginationParameters.PageNumber,
                request.PaginationParameters.PageSize
            )
        );
    }
}
