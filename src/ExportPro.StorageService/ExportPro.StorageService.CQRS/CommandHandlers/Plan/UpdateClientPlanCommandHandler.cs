using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.Plans;
public record UpdateClientPlanCommand(string clientId, string planId, PlansDto plansDto) : ICommand<ValidationModel<PlansResponse>>;
public class UpdateClientPlanCommandHandler(
    IClientRepository clientRepository
    ,IValidator<UpdateClientPlanCommand> validator) : ICommandHandler<UpdateClientPlanCommand, ValidationModel<PlansResponse>>
{
    private readonly IClientRepository _clientRepository=clientRepository;
    public async Task<BaseResponse<ValidationModel<PlansResponse>>> Handle(UpdateClientPlanCommand request, CancellationToken cancellationToken)
    {
        var validResult = await validator.ValidateAsync(request);
        if (!validResult.IsValid)
        {
            return new BaseResponse<ValidationModel<PlansResponse>>
            {
                Data = new(validResult),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var plan = await _clientRepository.UpdateClientPlan(request.clientId, request.planId, request.plansDto);
        return new BaseResponse<ValidationModel<PlansResponse>>
        {
            Data = new(plan),
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
        };
    }
}
