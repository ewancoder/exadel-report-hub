using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record SoftDeleteClientCommand(ObjectId ClientId) : ICommand<string>;

public class SoftDeleteClientCommandHandler(IClientRepository clientRepository) : ICommandHandler<SoftDeleteClientCommand, string>
{
    private IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<string>> Handle(SoftDeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetOneAsync(x=>x.Id==request.ClientId,CancellationToken.None);
        if (client == null)
            return new BaseResponse<string>
            {
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Client does not exist"],
                IsSuccess = false
            };
        var res = _clientRepository.SoftDeleteAsync(request.ClientId,CancellationToken.None);
        return new BaseResponse<string> { ApiState = HttpStatusCode.OK, Messages = ["Successfully deleted"], IsSuccess = true };
    }
}