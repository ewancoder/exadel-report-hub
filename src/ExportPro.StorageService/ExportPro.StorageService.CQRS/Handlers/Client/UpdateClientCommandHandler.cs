using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record UpdateClientCommand(ClientUpdateDto clientUpdateDto, string ClientId) : ICommand<ValidationModel<ClientResponse>>;

public class UpdateClientCommandHandler(IClientRepository clientRepository,IValidator<UpdateClientCommand> validator) : ICommandHandler<UpdateClientCommand, ValidationModel<ClientResponse>>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<ValidationModel<ClientResponse>>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<ValidationModel<ClientResponse>>
            {
                Data = new(validationResult),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var updatedClient = await _clientRepository.UpdateClient(request.clientUpdateDto, request.ClientId);
        
        return new BaseResponse<ValidationModel<ClientResponse>>
        {
            Data = new(updatedClient),
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
            Messages = ["Client Updated Successfully"],
        }; 
    }
}