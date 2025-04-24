using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.Plans;
using ExportPro.StorageService.CQRS.Commands.Items;
using ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;
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
public class ClientController(IMediator mediator, IHttpContextAccessor contextAccessor) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creating a client")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    [HasPermission(Common.Shared.Enums.Resource.Clients, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand clientCommand)
    {
        var clientResponse = await mediator.Send(clientCommand);
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Getting  clients")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    [HasPermission(Common.Shared.Enums.Resource.Clients, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetClients([FromQuery] int top = 5, [FromQuery] int skip = 0)
    {
        var clientResponse = await mediator.Send(new GetClientsQuery(top, skip));
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpGet("{clientId}")]
    [SwaggerOperation(Summary = "Getting  client by client id")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    [HasPermission(Common.Shared.Enums.Resource.Clients, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetClientById([Required] [FromRoute] string clientId)
    {
        var clientResponse = await mediator.Send(new GetClientByIdQuery(clientId));
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpPatch("{clientId}")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    [HasPermission(Common.Shared.Enums.Resource.Clients, Common.Shared.Enums.CrudAction.Update)]
    public async Task<IActionResult> UpdateClient([Required] string clientId, [FromBody] UpdateClientCommand command)
    {
        command = command with { clientId = clientId };
        var afterUpdate = await mediator.Send(command);
        return StatusCode((int)afterUpdate.ApiState, afterUpdate);
    }

    [HttpDelete("{clientId}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(BaseResponse<ClientResponse>), 200)]
    [HasPermission(Common.Shared.Enums.Resource.Clients, Common.Shared.Enums.CrudAction.Delete)]
    public async Task<IActionResult> SoftDeleteClient([FromRoute] string clientId)
    {
        var clientDeleting = await mediator.Send(new SoftDeleteClientCommand(clientId));
        return StatusCode((int)clientDeleting.ApiState, clientDeleting);
    }

    [HttpPatch("{clientId}/item")]
    [SwaggerOperation(Summary = "add single item to client")]
    [HasPermission(Common.Shared.Enums.Resource.Items, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> AddItemToClient(string clientId, [FromBody] ItemDtoForClient item)
    {
        var response = await mediator.Send(
            new CreateItemCommand(item.Name, item.Description, item.Price, item.Status, item.CurrencyId, clientId)
        );
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/items")]
    [SwaggerOperation(Summary = "add many items to client")]
    [HasPermission(Common.Shared.Enums.Resource.Items, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> AddItemsToClient(string clientId, [FromBody] List<ItemDtoForClient> items)
    {
        var response = await mediator.Send(new CreateItemsCommand(clientId, items));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{clientId}/item/{itemId}")]
    [SwaggerOperation(Summary = "remove item from client")]
    [HasPermission(Common.Shared.Enums.Resource.Items, Common.Shared.Enums.CrudAction.Delete)]
    public async Task<IActionResult> RemoveItemFromClient(string clientId, string itemId)
    {
        var response = await mediator.Send(new DeleteItemCommand(itemId, clientId));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{clientId}/item")]
    [SwaggerOperation(Summary = "update item in client")]
    [HasPermission(Common.Shared.Enums.Resource.Items, Common.Shared.Enums.CrudAction.Update)]
    public async Task<IActionResult> UpdateItemInClient(string clientId, [FromBody] Item item)
    {
        var response = await mediator.Send(new UpdateItemCommand(clientId, item));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{clientId}/items")]
    [SwaggerOperation(Summary = "update many items in client")]
    [HasPermission(Common.Shared.Enums.Resource.Items, Common.Shared.Enums.CrudAction.Update)]
    public async Task<IActionResult> UpdateItemsInClient(string clientId, [FromBody] List<Item> items)
    {
        var response = await mediator.Send(new UpdateItemsCommand(clientId, items));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/plan")]
    [SwaggerOperation(Summary = "add single plan to client")]
    [HasPermission(Common.Shared.Enums.Resource.Plans, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> AddPlanToClient([FromRoute] string clientId, [FromBody] PlansDto plan)
    {
        var response = await mediator.Send(new AddPlanToClientCommand(clientId, plan));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{clientId}/plan/{planId}")]
    [SwaggerOperation(Summary = "remove plan from client")]
    [HasPermission(Common.Shared.Enums.Resource.Plans, Common.Shared.Enums.CrudAction.Delete)]
    public async Task<IActionResult> RemovePlanFromClient([FromRoute] string clientId, [FromRoute] string planId)
    {
        var response = await mediator.Send(new RemovePlanFromClientCommand(clientId, planId));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/plan/{planId}")]
    [SwaggerOperation(Summary = "Update Client's Plan")]
    [HasPermission(Common.Shared.Enums.Resource.Plans, Common.Shared.Enums.CrudAction.Update)]
    public async Task<IActionResult> UpdateClientPlan(
        [FromRoute] string clientId,
        [FromRoute] string planId,
        [FromBody] PlansDto plansDto
    )
    {
        var response = await mediator.Send(new UpdateClientPlanCommand(clientId, planId, plansDto));
        return StatusCode((int)response.ApiState, response);
    }
}
