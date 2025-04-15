using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExportPro.StorageService.CQRS.Handlers.Client;

public record CreateClientCommand(ClientDto Clientdto) : ICommand<(ClientResponse?, Dictionary<string, string[]>?)>;

public class CreateClientCommandHandler(IClientRepository clientRepository, IValidator<CreateClientCommand> validator)
    : ICommandHandler<CreateClientCommand, (ClientResponse?, Dictionary<string, string[]>)>
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<BaseResponse<(ClientResponse?, Dictionary<string, string[]>)>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validation = validationResult
                .Errors.GroupBy(x => x.PropertyName.Replace("Clientdto.", ""))
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
            return new BaseResponse<(ClientResponse?, Dictionary<string, string[]>?)> { Data = (null, validation) };
        }
        var CreatingClient = await _clientRepository.AddClientFromClientDto(request.Clientdto);
        return new BaseResponse<(ClientResponse?, Dictionary<string, string[]>)>
        {
            Data = (CreatingClient, null),
            Messages = ["Client Created Successfully"],
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
        };
    }
}
