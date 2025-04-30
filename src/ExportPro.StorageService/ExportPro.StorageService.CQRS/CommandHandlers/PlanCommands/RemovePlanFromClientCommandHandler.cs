using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;

public record RemovePlanFromClientCommand(ObjectId PlanId) : ICommand<PlansResponse>;

public class RemovePlanFromClientCommandHandler(IClientRepository clientRepository)
    : ICommandHandler<RemovePlanFromClientCommand, PlansResponse>
{
    public async Task<BaseResponse<PlansResponse>> Handle(
        RemovePlanFromClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var plan = await clientRepository.RemovePlanFromClient(request.PlanId, cancellationToken);
        return new SuccessResponse<PlansResponse>(plan, "Plan removed from the client");
    }
}
