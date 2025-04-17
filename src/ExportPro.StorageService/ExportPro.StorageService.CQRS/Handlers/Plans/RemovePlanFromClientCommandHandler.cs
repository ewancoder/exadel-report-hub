using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.Handlers.Plans;

public record RemovePlanFromClientCommand(string clientId, string planId) : ICommand<ValidationModel<PlansResponse>>;

public class RemovePlanFromClientCommandHandler(
    IClientRepository clientRepository,
    IValidator<RemovePlanFromClientCommand> validator
) : ICommandHandler<RemovePlanFromClientCommand, ValidationModel<PlansResponse>>
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<BaseResponse<ValidationModel<PlansResponse>>> Handle(
        RemovePlanFromClientCommand request,
        CancellationToken cancellationToken
    )
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
        var plan = await _clientRepository.RemovePlanFromClient(request.clientId, request.planId);
        return new BaseResponse<ValidationModel<PlansResponse>>
        {
            Data = new(plan),
            ApiState = HttpStatusCode.BadRequest,
            IsSuccess = false,
        };
    }
}
