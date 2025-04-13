using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Commands;

public record UpdateClientCommand(ClientUpdateDto clientUpdateDto,string ClientId):ICommand<ClientResponse>;
