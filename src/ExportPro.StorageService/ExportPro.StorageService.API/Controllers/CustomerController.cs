using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

 
    [HttpPut("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid customer ID format.");

        command.Id = id;
        return Ok(await mediator.Send(command, cancellationToken));
    }
  
    [HttpDelete("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid customer ID format.");

        return Ok(await mediator.Send(new DeleteCustomerCommand(objectId), cancellationToken));
    }
    
    [HttpGet("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid customer ID format.");

        return Ok(await mediator.Send(new GetCustomerByIdQuery(objectId), cancellationToken));
    }

    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Read)]
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedList<Customer>>>> GetAll(
        CancellationToken cancellationToken, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] bool includeDeleted = false)
    {
        var query = new GetPaginatedCustomersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IncludeDeleted = includeDeleted
        };
        var response = await mediator.Send(query);
        return StatusCode((int)response.ApiState, response);
    }
}