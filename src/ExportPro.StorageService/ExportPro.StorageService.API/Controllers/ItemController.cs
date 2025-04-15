using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Commands.Items;
using ExportPro.StorageService.CQRS.Queries.Items;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    // GET: api/item
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _mediator.Send(new GetItemsQuery());
        return StatusCode((int)response.ApiState, response);
    }

    // GET: api/item/filter?clientId=...&invoiceId=...&customerId=...
    [HttpGet("filter")]
    public async Task<IActionResult> GetFiltered([FromQuery] string? clientId, [FromQuery] string? invoiceId, [FromQuery] string? customerId)
    {
        var response = await _mediator.Send(new GetFilteredItemsQuery(customerId, clientId, invoiceId));
        return StatusCode((int)response.ApiState, response);
    }

    // POST: api/item
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemCommand command)
    {
        var response = await _mediator.Send(command);
        return StatusCode((int)response.ApiState, response);
    }

    // POST: api/item/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulk([FromBody] CreateItemsCommand command)
    {
        var response = await _mediator.Send(command);
        return StatusCode((int)response.ApiState, response);
    }

    // PUT: api/item/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateItemCommand command)
    {
        if (id != command.Id.ToString())
            return BadRequest(new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = new List<string> { "ID mismatch between route and payload." }
            });

        var response = await _mediator.Send(command);
        return StatusCode((int)response.ApiState, response);
    }

    // PUT: api/item/bulk
    [HttpPut("bulk")]
    public async Task<IActionResult> UpdateBulk([FromBody] UpdateItemsCommand command)
    {
        var response = await _mediator.Send(command);
        return StatusCode((int)response.ApiState, response);
    }

    // DELETE: api/item/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return BadRequest(new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = new List<string> { "Invalid ObjectId format." }
            });
        }

        var response = await _mediator.Send(new DeleteItemCommand(objectId));
        return StatusCode((int)response.ApiState, response);
    }

}

