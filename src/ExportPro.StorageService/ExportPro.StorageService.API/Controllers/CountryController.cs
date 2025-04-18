using ExportPro.StorageService.CQRS.Commands.CountryCommand;
using ExportPro.StorageService.CQRS.Queries.CountryQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateCountryCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ObjectId id, [FromBody] UpdateCountryCommand command)
        => Ok(await _mediator.Send(command with { Id = id }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(ObjectId id)
        => Ok(await _mediator.Send(new DeleteCountryCommand(id)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(ObjectId id)
        => Ok(await _mediator.Send(new GetCountryByIdQuery(id)));

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new GetPaginatedCountriesQuery()));
}
