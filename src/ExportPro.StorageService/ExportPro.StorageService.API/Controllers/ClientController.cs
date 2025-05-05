using System.ComponentModel.DataAnnotations;
using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;
using ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.StorageService.API.Controllers;

[Route("api/client/")]
[Authorize]
[ApiController]
public class ClientController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creating a client")]
    [ProducesResponseType(typeof(NotFoundResponse<ClientResponse>), 404)]
    [HasPermission(Resource.Clients, CrudAction.Create)]
    public Task<BaseResponse<ClientResponse>> CreateClient([FromBody] CreateClientCommand clientCommand) =>
        mediator.Send(clientCommand);

    [HttpGet]
    [SwaggerOperation(Summary = "Getting  clients")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    [HasPermission(Resource.Clients, CrudAction.Read)]
    public Task<BaseResponse<List<ClientResponse>>> GetClients([FromQuery] int top = 5, [FromQuery] int skip = 0) =>
        mediator.Send(new GetClientsQuery(top, skip));

    [HttpGet("{clientId}")]
    [SwaggerOperation(Summary = "Getting  client by client id")]
    [ProducesResponseType(typeof(SuccessResponse<ClientResponse>), 200)]
    [ProducesResponseType(typeof(NotFoundResponse<ClientResponse>), 404)]
    [HasPermission(Resource.Clients, CrudAction.Read)]
    public Task<BaseResponse<ClientResponse>> GetClientById([Required] [FromRoute] Guid clientId) =>
        mediator.Send(new GetClientByIdQuery(clientId));

    [HttpPatch("{clientId}")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    [HasPermission(Resource.Clients, CrudAction.Update)]
    public Task<BaseResponse<ClientResponse>> UpdateClient([FromRoute] Guid clientId, [FromBody] ClientDto client) =>
        mediator.Send(new UpdateClientCommand(client, clientId));

    [HttpDelete("{clientId}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(BaseResponse<ClientResponse>), 200)]
    [HasPermission(Resource.Clients, CrudAction.Delete)]
    public Task<BaseResponse<ClientResponse>> SoftDeleteClient([FromRoute] Guid clientId) =>
        mediator.Send(new SoftDeleteClientCommand(clientId.ToObjectId()));

    [HttpPatch("{clientId}/item")]
    [SwaggerOperation(Summary = "add single item to client")]
    [HasPermission(Resource.Items, CrudAction.Create)]
    public Task<BaseResponse<string>> AddItemToClient(Guid clientId, [FromBody] ItemDtoForClient item) =>
        mediator.Send(
            new CreateItemCommand(item.Name, item.Description, item.Price, item.Status, item.CurrencyId, clientId)
        );

    [HttpPatch("{clientId}/items")]
    [SwaggerOperation(Summary = "add many items to client")]
    [HasPermission(Resource.Items, CrudAction.Create)]
    public Task<BaseResponse<bool>> AddItemsToClient(Guid clientId, [FromBody] List<ItemDtoForClient> items) =>
        mediator.Send(new CreateItemsCommand(clientId, items));

    [HttpDelete("{clientId}/item/{itemId}")]
    [SwaggerOperation(Summary = "remove item from client")]
    [HasPermission(Resource.Items, CrudAction.Delete)]
    public Task<BaseResponse<bool>> RemoveItemFromClient(Guid clientId, Guid itemId) =>
        mediator.Send(new DeleteItemCommand(itemId, clientId));

    [HttpPut("{clientId}/item")]
    [SwaggerOperation(Summary = "update item in client")]
    [HasPermission(Resource.Items, CrudAction.Update)]
    public Task<BaseResponse<bool>> UpdateItemInClient(Guid clientId, [FromBody] Item item) =>
        mediator.Send(new UpdateItemCommand(clientId, item));

    [HttpPut("{clientId}/items")]
    [SwaggerOperation(Summary = "update many items in client")]
    [HasPermission(Resource.Items, CrudAction.Update)]
    public Task<BaseResponse<bool>> UpdateItemsInClient(Guid clientId, [FromBody] List<Item> items) =>
        mediator.Send(new UpdateItemsCommand(clientId, items));

    [HttpGet("plan/{planId}")]
    [SwaggerOperation(Summary = "Get Plan by id ")]
    [HasPermission(Resource.Plans, CrudAction.Read)]
    public Task<BaseResponse<PlansResponse>> GetPlan(Guid planId) => mediator.Send(new GetPlanQuery(planId));

    [HttpGet("{clientId}/plans")]
    [SwaggerOperation(Summary = "Get Client Plans")]
    [HasPermission(Resource.Plans, CrudAction.Read)]
    public Task<BaseResponse<List<PlansResponse>>> GetClientPlans(
        [FromRoute] Guid clientId,
        [FromQuery] int top = 5,
        [FromQuery] int skip = 0
    ) => mediator.Send(new GetClientPlansQuery(clientId, top, skip));

    [HttpPatch("{clientId}/plan")]
    [SwaggerOperation(Summary = "add single plan to client")]
    [HasPermission(Resource.Plans, CrudAction.Create)]
    public Task<BaseResponse<PlansResponse>> AddPlanToClient([FromRoute] Guid clientId, [FromBody] PlansDto plan) =>
        mediator.Send(new AddPlanToClientCommand(clientId, plan));

    [HttpDelete("plan/{planId}")]
    [SwaggerOperation(Summary = "remove plan from client")]
    [HasPermission(Resource.Plans, CrudAction.Delete)]
    public Task<BaseResponse<PlansResponse>> RemovePlanFromClient([FromRoute] Guid planId) =>
        mediator.Send(new RemovePlanFromClientCommand(planId.ToObjectId()));

    [HttpPatch("plan/{planId}")]
    [SwaggerOperation(Summary = "Update Client's Plan")]
    [HasPermission(Resource.Plans, CrudAction.Update)]
    public Task<BaseResponse<PlansResponse>> UpdateClientPlan([FromRoute] Guid planId, [FromBody] PlansDto plansDto) =>
        mediator.Send(new UpdateClientPlanCommand(planId, plansDto));
}
