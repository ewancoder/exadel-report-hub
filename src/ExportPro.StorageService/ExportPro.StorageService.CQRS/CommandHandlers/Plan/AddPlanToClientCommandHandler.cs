using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.CommandHandlers.Plans;

public record AddPlanToClientCommand(string clientId, PlansDto plan) : ICommand<ValidationModel<PlansResponse>>;

public class AddPlanToClientCommandHandler(
    IClientRepository clientRepository,
    IMapper mapper,
    IValidator<AddPlanToClientCommand> validator
) : ICommandHandler<AddPlanToClientCommand, ValidationModel<PlansResponse>>
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<BaseResponse<ValidationModel<PlansResponse>>> Handle(
        AddPlanToClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<ValidationModel<PlansResponse>>
            {
                Data = new(validationResult),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var Plan = await _clientRepository.AddPlanToClient(request.clientId, request.plan);
        var plan = mapper.Map<PlansResponse>(Plan);
        return new BaseResponse<ValidationModel<PlansResponse>>
        {
            Data = new(plan),
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
        };
    }
}
