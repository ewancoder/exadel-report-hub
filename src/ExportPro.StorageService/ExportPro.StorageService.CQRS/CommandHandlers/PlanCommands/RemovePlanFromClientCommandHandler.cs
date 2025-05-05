using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;

public sealed record RemovePlanFromClientCommand(ObjectId PlanId) : ICommand<PlansResponse>;

public sealed class RemovePlanFromClientCommandHandler(IClientRepository clientRepository)
    : ICommandHandler<RemovePlanFromClientCommand, PlansResponse>
{
    public async Task<BaseResponse<PlansResponse>> Handle(
        RemovePlanFromClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var plansResponse = await clientRepository.GetPlan(request.PlanId, cancellationToken);
        if (plansResponse == null)
            return new NotFoundResponse<PlansResponse>("Plan Not Found");
        var plan = await clientRepository.RemovePlanFromClient(request.PlanId, cancellationToken);
        return new SuccessResponse<PlansResponse>(plan, "Plan removed from the client");
    }
}
