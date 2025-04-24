using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;

public record AddPlanToClientCommand(string ClientId, PlansDto Plan) : ICommand<PlansResponse>;

public class AddPlanToClientCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IClientRepository clientRepository,
    IMapper mapper
) : ICommandHandler<AddPlanToClientCommand, PlansResponse>
{
    public async Task<BaseResponse<PlansResponse>> Handle(
        AddPlanToClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var plan = await clientRepository.AddPlanToClient(request.ClientId, request.Plan, cancellationToken);
        var planResponse = mapper.Map<PlansResponse>(plan);
        return new SuccessResponse<PlansResponse>(planResponse, "Added plan to the client");
    }
}
