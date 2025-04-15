using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record DeleteClientCommand(ObjectId ClientId) : ICommand<string>;
public class DeleteClientCommandHandler(IClientRepository clientRepository) : ICommandHandler<DeleteClientCommand, string>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<string>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var message = await _clientRepository.DeleteClient(request.ClientId);
        if (message == null)
            return null;
        return new SuccessResponse<string>(message);
    }
}