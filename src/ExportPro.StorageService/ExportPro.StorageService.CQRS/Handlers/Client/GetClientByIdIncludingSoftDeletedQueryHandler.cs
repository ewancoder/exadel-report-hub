using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record GetClientByIdIncludingSoftDeletedQuery(ObjectId Id) : IQuery<ClientResponse>;

public class GetClientByIdIncludingSoftDeletedQueryHandler(IClientRepository clientRepository) : IQueryHandler<GetClientByIdIncludingSoftDeletedQuery, ClientResponse>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<ClientResponse>> Handle(GetClientByIdIncludingSoftDeletedQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetClientByIdIncludingSoftDeleted(request.Id);
        if (client == null)
            return null;
        return new SuccessResponse<ClientResponse>(client);
    }
}