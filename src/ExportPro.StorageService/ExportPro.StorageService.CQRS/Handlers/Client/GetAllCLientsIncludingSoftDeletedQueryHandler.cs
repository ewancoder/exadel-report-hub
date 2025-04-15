using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record GetAllCLientsIncludingSoftDeletedQuery : IQuery<List<ClientResponse>>;

public class GetAllCLientsIncludingSoftDeletedQueryHandler(IClientRepository clientRepository) : IQueryHandler<GetAllCLientsIncludingSoftDeletedQuery, List<ClientResponse>>
{
    IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<List<ClientResponse>>> Handle(GetAllCLientsIncludingSoftDeletedQuery request, CancellationToken cancellationToken)
    {
        var clients = await _clientRepository.GetAllCLientsIncludingSoftDeleted();
        return new SuccessResponse<List<ClientResponse>>(clients, message: "Clients are sucessfully retrieved");
    }
}