using System.ComponentModel.DataAnnotations;
using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;
using ExportPro.StorageService.CQRS.QueryHandlers.ItemQueries;
using ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [ProducesResponseType(typeof(ClientResponse), 200)]
    [HasPermission(Resource.Clients, CrudAction.Create)]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand clientCommand)
    {
        var clientResponse = await mediator.Send(clientCommand);
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Getting  clients")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    [HasPermission(Resource.Clients, CrudAction.Read)]
    public async Task<IActionResult> GetClients([FromQuery] int top = 5, [FromQuery] int skip = 0)
    {
        var clientResponse = await mediator.Send(new GetClientsQuery(top, skip));
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpGet("{clientId}")]
    [SwaggerOperation(Summary = "Getting  client by client id")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    [HasPermission(Resource.Clients, CrudAction.Read)]
    public async Task<IActionResult> GetClientById([Required] [FromRoute] Guid clientId)
    {
        var clientResponse = await mediator.Send(new GetClientByIdQuery(clientId));
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpPatch("{clientId}")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    [HasPermission(Resource.Clients, CrudAction.Update)]
    public async Task<IActionResult> UpdateClient([FromRoute] Guid clientId, [FromBody] ClientDto client)
    {
        var afterUpdate = await mediator.Send(new UpdateClientCommand(client, clientId));
        return StatusCode((int)afterUpdate.ApiState, afterUpdate);
    }

    [HttpDelete("{clientId}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(BaseResponse<ClientResponse>), 200)]
    [HasPermission(Resource.Clients, CrudAction.Delete)]
    public async Task<IActionResult> SoftDeleteClient([FromRoute] Guid clientId)
    {
        var clientDeleting = await mediator.Send(new SoftDeleteClientCommand(clientId.ToObjectId()));
        return StatusCode((int)clientDeleting.ApiState, clientDeleting);
    }

    //[AllowAnonymous]
    [HttpGet("{clientId}/items")]
    [SwaggerOperation(Summary = "Get all items for a client")]
    [ProducesResponseType(typeof(List<ItemResponse>), 200)]
    [HasPermission(Common.Shared.Enums.Resource.Items, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetItems(
            [FromRoute] Guid clientId,
            CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetItemsQuery(clientId), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{clientId}/items/{itemId}")]
    [SwaggerOperation(Summary = "Get a single item by ID for a client")]
    [ProducesResponseType(typeof(ItemResponse), 200)]
    [HasPermission(Common.Shared.Enums.Resource.Items, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetItemById(
            [FromRoute] Guid clientId,
            [FromRoute] Guid itemId,
            CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetItemByIdQuery(clientId, itemId), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/item")]
    [SwaggerOperation(Summary = "add single item to client")]
    [HasPermission(Resource.Items, CrudAction.Create)]
    public async Task<IActionResult> AddItemToClient(Guid clientId, [FromBody] ItemDtoForClient item)
    {
        var response = await mediator.Send(
            new CreateItemCommand(item.Name, item.Description, item.Price, item.Status, item.CurrencyId, clientId)
        );
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/items")]
    [SwaggerOperation(Summary = "add many items to client")]
    [HasPermission(Resource.Items, CrudAction.Create)]
    public async Task<IActionResult> AddItemsToClient(Guid clientId, [FromBody] List<ItemDtoForClient> items)
    {
        var response = await mediator.Send(new CreateItemsCommand(clientId, items));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{clientId}/item/{itemId}")]
    [SwaggerOperation(Summary = "remove item from client")]
    [HasPermission(Resource.Items, CrudAction.Delete)]
    public async Task<IActionResult> RemoveItemFromClient(Guid clientId, Guid itemId)
    {
        var response = await mediator.Send(new DeleteItemCommand(itemId, clientId));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{clientId}/item")]
    [SwaggerOperation(Summary = "update item in client")]
    [HasPermission(Resource.Items, CrudAction.Update)]
    public async Task<IActionResult> UpdateItemInClient(Guid clientId, [FromBody] Item item)
    {
        var response = await mediator.Send(new UpdateItemCommand(clientId, item));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{clientId}/items")]
    [SwaggerOperation(Summary = "update many items in client")]
    [HasPermission(Resource.Items, CrudAction.Update)]
    public async Task<IActionResult> UpdateItemsInClient(Guid clientId, [FromBody] List<Item> items)
    {
        var response = await mediator.Send(new UpdateItemsCommand(clientId, items));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("plan/{planId}")]
    [SwaggerOperation(Summary = "Get Plan by id ")]
    [HasPermission(Resource.Plans, CrudAction.Read)]
    public async Task<IActionResult> GetPlan(Guid planId)
    {
        var response = await mediator.Send(new GetPlanQuery(planId));
        return StatusCode((int)response.ApiState, response);
    }

    //[AllowAnonymous]
    [HttpGet("{clientId}/plans")]
    [SwaggerOperation(Summary = "Get Client Plans")]
    [HasPermission(Resource.Plans, CrudAction.Read)]
    public async Task<IActionResult> GetClientPlans(
        [FromRoute] Guid clientId,
        [FromQuery] int top = 5,
        [FromQuery] int skip = 0
    )
    {
        var response = await mediator.Send(new GetClientPlansQuery(clientId, top, skip));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/plan")]
    [SwaggerOperation(Summary = "add single plan to client")]
    [HasPermission(Resource.Plans, CrudAction.Create)]
    public async Task<IActionResult> AddPlanToClient([FromRoute] Guid clientId, [FromBody] PlansDto plan)
    {
        var response = await mediator.Send(new AddPlanToClientCommand(clientId, plan));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("plan/{planId}")]
    [SwaggerOperation(Summary = "remove plan from client")]
    [HasPermission(Resource.Plans, CrudAction.Delete)]
    public async Task<IActionResult> RemovePlanFromClient([FromRoute] Guid planId)
    {
        var response = await mediator.Send(new RemovePlanFromClientCommand(planId.ToObjectId()));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("plan/{planId}")]
    [SwaggerOperation(Summary = "Update Client's Plan")]
    [HasPermission(Resource.Plans, CrudAction.Update)]
    public async Task<IActionResult> UpdateClientPlan([FromRoute] Guid planId, [FromBody] PlansDto plansDto)
    {
        var response = await mediator.Send(new UpdateClientPlanCommand(planId, plansDto));
        return StatusCode((int)response.ApiState, response);
    }
}
