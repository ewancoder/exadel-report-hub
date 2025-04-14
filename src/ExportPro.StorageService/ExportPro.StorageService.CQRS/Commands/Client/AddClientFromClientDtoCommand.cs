using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Commands.Client;

public record AddClientFromClientDtoCommand(ClientDto Clientdto) : ICommand<ClientResponse>;
