using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;

public record UpdateClientPlanCommand(string PlanId, PlansDto PlansDto) : ICommand<ValidationModel<PlansResponse>>;

public class UpdateClientPlanCommandHandler(IClientRepository clientRepository)
    : ICommandHandler<UpdateClientPlanCommand, ValidationModel<PlansResponse>>
{
    public async Task<BaseResponse<ValidationModel<PlansResponse>>> Handle(
        UpdateClientPlanCommand request,
        CancellationToken cancellationToken
    )
    {
        var plan = await clientRepository.UpdateClientPlan(request.PlanId, request.PlansDto, cancellationToken);
        return new BaseResponse<ValidationModel<PlansResponse>>
        {
            Data = new(plan),
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
        };
    }
}
