using ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.CountryQueries;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCountryCommand command,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(command, cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCountry country,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new UpdateCountryCommand(id, country), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteCountryCommand(id), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCountryByIdQuery(id), cancellationToken);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeDeleted = false
    )
    {
        var response = await mediator.Send(
            new GetPaginatedCountriesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                IncludeDeleted = includeDeleted,
            },
            cancellationToken
        );

        return StatusCode((int)response.ApiState, response);
    }
}
