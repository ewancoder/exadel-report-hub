using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;

public sealed record GetClientByIdQuery(Guid ClientId) : IQuery<ClientResponse>;

public sealed class GetClientByIdQueryHandler(IClientRepository clientRepository, IMapper mapper)
    : IQueryHandler<GetClientByIdQuery, ClientResponse>
{
    public async Task<BaseResponse<ClientResponse>> Handle(
        GetClientByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (client == null)
            return new NotFoundResponse<ClientResponse>("Client Not Found");
        var clientResponse = mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Retrieved");
    }
}
