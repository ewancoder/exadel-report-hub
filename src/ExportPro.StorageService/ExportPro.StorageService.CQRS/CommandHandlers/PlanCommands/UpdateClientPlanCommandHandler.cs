using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;

public sealed record UpdateClientPlanCommand(Guid PlanId, PlansDto PlansDto) : ICommand<PlansResponse>;

public sealed class UpdateClientPlanCommandHandler(IClientRepository clientRepository)
    : ICommandHandler<UpdateClientPlanCommand, PlansResponse>
{
    public async Task<BaseResponse<PlansResponse>> Handle(
        UpdateClientPlanCommand request,
        CancellationToken cancellationToken
    )
    {
        var plansResponse = await clientRepository.GetPlan(request.PlanId.ToObjectId(), cancellationToken);
        if (plansResponse == null)
            return new NotFoundResponse<PlansResponse>("Plan Not Found");
        var plan = await clientRepository.UpdateClientPlan(
            request.PlanId.ToObjectId(),
            request.PlansDto,
            cancellationToken
        );
        return new SuccessResponse<PlansResponse>(plan!, "Plan updated successfully");
    }
}
