using System.ComponentModel.DataAnnotations;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;
using ExportPro.StorageService.CQRS.QueryHandlers.ItemQueries;
using ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.StorageService.API.Controllers;

[Route("api/client/")]
[Authorize]
[ApiController]
public class ClientController(IMediator mediator) : ControllerBase, IClientController
{
    [HttpPost]
    [SwaggerOperation(Summary = "Creating a client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    [ProducesResponseType(typeof(NotFoundResponse<ClientResponse>), 404)]
    public Task<BaseResponse<ClientResponse>> CreateClient(
        [FromBody] ClientDto client,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new CreateClientCommand(client), cancellationToken);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Getting  clients")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public Task<BaseResponse<PaginatedList<ClientResponse>>> GetClients(
        [FromQuery] PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new GetClientsQuery(paginationParameters), cancellationToken);
    }

    [HttpGet("{clientId}")]
    [SwaggerOperation(Summary = "Getting  client by client id")]
    [ProducesResponseType(typeof(SuccessResponse<ClientResponse>), 200)]
    [ProducesResponseType(typeof(NotFoundResponse<ClientResponse>), 404)]
    public Task<BaseResponse<ClientResponse>> GetClientById(
        [Required] [FromRoute] Guid clientId,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new GetClientByIdQuery(clientId), cancellationToken);
    }

    [HttpPatch("{clientId}")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public Task<BaseResponse<ClientResponse>> UpdateClient(
        [FromRoute] Guid clientId,
        [FromBody] ClientDto client,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new UpdateClientCommand(client, clientId), cancellationToken);
    }

    [HttpDelete("{clientId}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(BaseResponse<ClientResponse>), 200)]
    public Task<BaseResponse<ClientResponse>> SoftDeleteClient(
        [FromRoute] Guid clientId,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new SoftDeleteClientCommand(clientId), cancellationToken);
    }

    [HttpGet("{clientId}/items")]
    [SwaggerOperation(Summary = "Get all items of a client")]
    [ProducesResponseType(typeof(List<ItemResponse>), 200)]
    public Task<BaseResponse<PaginatedList<ItemResponse>>> GetClientItems(
        [FromRoute] Guid clientId,
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new GetClientItemsQuery(clientId, parameters), cancellationToken);
    }

    [HttpPatch("{clientId}/item")]
    [SwaggerOperation(Summary = "add single item to client")]
    public Task<BaseResponse<Guid>> AddItemToClient(
        Guid clientId,
        [FromBody] ItemDtoForClient item,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(
            new CreateItemCommand(item.Name, item.Description, item.Price, item.Status, item.CurrencyId, clientId),
            cancellationToken
        );
    }

    [HttpPatch("{clientId}/items")]
    [SwaggerOperation(Summary = "add many items to client")]
    public Task<BaseResponse<bool>> AddItemsToClient(
        Guid clientId,
        [FromBody] List<ItemDtoForClient> items,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new CreateItemsCommand(clientId, items), cancellationToken);
    }

    [HttpDelete("{clientId}/item/{itemId}")]
    [SwaggerOperation(Summary = "remove item from client")]
    public Task<BaseResponse<bool>> RemoveItemFromClient(
        Guid clientId,
        Guid itemId,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new DeleteItemCommand(itemId, clientId), cancellationToken);
    }

    [HttpPut("{clientId}/item")]
    [SwaggerOperation(Summary = "update item in client")]
    public Task<BaseResponse<bool>> UpdateItemInClient(
        Guid clientId,
        [FromBody] Item item,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new UpdateItemCommand(clientId, item), cancellationToken);
    }

    [HttpPut("{clientId}/items")]
    [SwaggerOperation(Summary = "update many items in client")]
    public Task<BaseResponse<bool>> UpdateItemsInClient(
        Guid clientId,
        [FromBody] List<Item> items,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new UpdateItemsCommand(clientId, items), cancellationToken);
    }

    [HttpGet("plan/{planId}")]
    [SwaggerOperation(Summary = "Get Plan by id ")]
    public Task<BaseResponse<PlansResponse>> GetPlan(Guid planId, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetPlanQuery(planId), cancellationToken);
    }

    [HttpGet("{clientId}/plans")]
    [SwaggerOperation(Summary = "Get Client Plans")]
    public Task<BaseResponse<PaginatedList<PlansResponse>>> GetClientPlans(
        [FromRoute] Guid clientId,
        [FromQuery] PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new GetClientPlansQuery(clientId, paginationParameters), cancellationToken);
    }

    [HttpPatch("{clientId}/plan")]
    [SwaggerOperation(Summary = "add single plan to client")]
    public Task<BaseResponse<PlansResponse>> AddPlanToClient(
        [FromRoute] Guid clientId,
        [FromBody] PlansDto plan,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new AddPlanToClientCommand(clientId, plan), cancellationToken);
    }

    [HttpDelete("plan/{planId}")]
    [SwaggerOperation(Summary = "remove plan from client")]
    public Task<BaseResponse<PlansResponse>> RemovePlanFromClient(
        [FromRoute] Guid planId,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new RemovePlanFromClientCommand(planId.ToObjectId()), cancellationToken);
    }

    [HttpPatch("plan/{planId}")]
    [SwaggerOperation(Summary = "Update Client's Plan")]
    public Task<BaseResponse<PlansResponse>> UpdateClientPlan(
        [FromRoute] Guid planId,
        [FromBody] PlansDto plansDto,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new UpdateClientPlanCommand(planId, plansDto), cancellationToken);
    }

    [HttpGet("{clientId}/items/invoice")]
    [SwaggerOperation(Summary = "Get all items of a invoice a client")]
    [ProducesResponseType(typeof(List<ItemResponse>), 200)]
    public Task<BaseResponse<List<ItemResponse>>> GetItems(
        [FromRoute] Guid clientId,
        CancellationToken cancellationToken
    ) => mediator.Send(new GetItemsQuery(clientId), cancellationToken);

    [HttpGet("{clientId}/items/{itemId}")]
    [SwaggerOperation(Summary = "Get a single item by ID for a client")]
    [ProducesResponseType(typeof(ItemResponse), 200)]
    public Task<BaseResponse<ItemResponse>> GetItemById(
        [FromRoute] Guid clientId,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken
    ) => mediator.Send(new GetItemByIdQuery(clientId, itemId), cancellationToken);
}
