using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;

public sealed record AddPlanToClientCommand(Guid ClientId, PlansDto Plan) : ICommand<PlansResponse>;

public sealed class AddPlanToClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    : ICommandHandler<AddPlanToClientCommand, PlansResponse>
{
    public async Task<BaseResponse<PlansResponse>> Handle(
        AddPlanToClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var plan = await clientRepository.AddPlanToClient(
            request.ClientId.ToObjectId(),
            request.Plan,
            cancellationToken
        );
        var planResponse = mapper.Map<PlansResponse>(plan);
        return new SuccessResponse<PlansResponse>(planResponse, "Added plan to the client");
    }
}
