using System.ComponentModel.DataAnnotations;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.Items;
using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;
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
public class ClientController(IMediator mediator, IHttpContextAccessor contextAccessor) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creating a client")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand clientCommand)
    {
        var clientResponse = await mediator.Send(clientCommand);
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Getting  clients")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetClients([FromQuery] int top = 5, [FromQuery] int skip = 0)
    {
        var clientResponse = await mediator.Send(new GetClientsQuery(top, skip));
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpGet("{clientId}")]
    [SwaggerOperation(Summary = "Getting  client by client id")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    public async Task<IActionResult> GetClientById([Required] [FromRoute] string clientId)
    {
        var clientResponse = await mediator.Send(new GetClientByIdQuery(clientId));
        return StatusCode((int)clientResponse.ApiState, clientResponse);
    }

    [HttpPatch("{clientId}")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> UpdateClient([FromRoute] string clientId, [FromBody] ClientDto client)
    {
        var afterUpdate = await mediator.Send(new UpdateClientCommand(client, clientId));
        return StatusCode((int)afterUpdate.ApiState, afterUpdate);
    }

    [HttpDelete("{clientId}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(BaseResponse<ClientResponse>), 200)]
    public async Task<IActionResult> SoftDeleteClient([FromRoute] string clientId)
    {
        var clientDeleting = await mediator.Send(new SoftDeleteClientCommand(clientId));
        return StatusCode((int)clientDeleting.ApiState, clientDeleting);
    }

    [HttpPatch("{clientId}/item")]
    [SwaggerOperation(Summary = "add single item to client")]
    public async Task<IActionResult> AddItemToClient(string clientId, [FromBody] ItemDtoForClient item)
    {
        var response = await mediator.Send(
            new CreateItemCommand(item.Name, item.Description, item.Price, item.Status, item.CurrencyId, clientId)
        );
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/items")]
    [SwaggerOperation(Summary = "add many items to client")]
    public async Task<IActionResult> AddItemsToClient(string clientId, [FromBody] List<ItemDtoForClient> items)
    {
        var response = await mediator.Send(new CreateItemsCommand(clientId, items));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{clientId}/item/{itemId}")]
    [SwaggerOperation(Summary = "remove item from client")]
    public async Task<IActionResult> RemoveItemFromClient(string clientId, string itemId)
    {
        var response = await mediator.Send(new DeleteItemCommand(itemId, clientId));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{clientId}/item")]
    [SwaggerOperation(Summary = "update item in client")]
    public async Task<IActionResult> UpdateItemInClient(string clientId, [FromBody] Item item)
    {
        var response = await mediator.Send(new UpdateItemCommand(clientId, item));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{clientId}/items")]
    [SwaggerOperation(Summary = "update many items in client")]
    public async Task<IActionResult> UpdateItemsInClient(string clientId, [FromBody] List<Item> items)
    {
        var response = await mediator.Send(new UpdateItemsCommand(clientId, items));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("plan/{planId}")]
    [SwaggerOperation(Summary = "Get Plan by id ")]
    public async Task<IActionResult> GetPlan(string planId)
    {
        var response = await mediator.Send(new GetPlanQuery(planId));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{clientId}/plans")]
    [SwaggerOperation(Summary = "Get Client Plans")]
    public async Task<IActionResult> GetClientPlans(
        [FromRoute] string clientId,
        [FromQuery] int top = 5,
        [FromQuery] int skip = 0
    )
    {
        var response = await mediator.Send(new GetClientPlansQuery(clientId, top, skip));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("{clientId}/plan")]
    [SwaggerOperation(Summary = "add single plan to client")]
    public async Task<IActionResult> AddPlanToClient([FromRoute] string clientId, [FromBody] PlansDto plan)
    {
        var response = await mediator.Send(new AddPlanToClientCommand(clientId, plan));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("plan/{planId}")]
    [SwaggerOperation(Summary = "remove plan from client")]
    public async Task<IActionResult> RemovePlanFromClient([FromRoute] string planId)
    {
        var response = await mediator.Send(new RemovePlanFromClientCommand(planId));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPatch("plan/{planId}")]
    [SwaggerOperation(Summary = "Update Client's Plan")]
    public async Task<IActionResult> UpdateClientPlan([FromRoute] string planId, [FromBody] PlansDto plansDto)
    {
        var response = await mediator.Send(new UpdateClientPlanCommand(planId, plansDto));
        return StatusCode((int)response.ApiState, response);
    }
}
