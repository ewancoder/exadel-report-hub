using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;
namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record SoftDeleteClientCommand(string ClientId) : ICommand<ValidationModel<ClientResponse>>;

public class SoftDeleteClientCommandHandler(IClientRepository clientRepository,
    IMapper _mapper,IValidator<SoftDeleteClientCommand> validator) : ICommandHandler<SoftDeleteClientCommand, ValidationModel<ClientResponse>>
{
    private IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<ValidationModel<ClientResponse>>> Handle(SoftDeleteClientCommand request, CancellationToken cancellationToken)
    {
        var validres = await validator.ValidateAsync(request,CancellationToken.None);
        if (!validres.IsValid)
        {
            return new BaseResponse<ValidationModel<ClientResponse>>
            {
                Data = new ValidationModel<ClientResponse>(validres),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var client = await _clientRepository.GetClientById(request.ClientId);
        await _clientRepository.SoftDeleteClient(request.ClientId);
        return new BaseResponse<ValidationModel<ClientResponse>>
        {
            Data = new(_mapper.Map<ClientResponse>(client)),
            ApiState = HttpStatusCode.OK, Messages = ["Successfully deleted"], IsSuccess = true
        };
    }
}