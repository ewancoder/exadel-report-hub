using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using ZstdSharp.Unsafe;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record GetClientByIdQuery(string Id) : IQuery<ClientResponse>;

public class GetClientByIdQueryHandler(IClientRepository clientRepository,IMapper mapper) : IQueryHandler<GetClientByIdQuery, ClientResponse>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IMapper _mapper = mapper;
    public async Task<BaseResponse<ClientResponse>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetClientById(request.Id);
        var clientrep = _mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientrep, message: "Client found");
    }
}