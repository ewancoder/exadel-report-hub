using ExportPro.StorageService.CQRS.Commands.CountryCommand;
using ExportPro.StorageService.CQRS.Queries.CountryQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCountryCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCountryCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
            return BadRequest("Invalid country ID format.");

        command = command with { Id = id };
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid country ID format.");

        var response = await _mediator.Send(new DeleteCountryCommand(id), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetCountryByIdQuery(id), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool includeDeleted = false)
    {
        var response = await _mediator.Send(
            new GetPaginatedCountriesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                IncludeDeleted = includeDeleted
            },
            cancellationToken);

        return StatusCode((int)response.ApiState, response);
    }
}
