using ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        command.Id = ObjectId.Parse(id); // No need for try/catch as FluentValidation will handle it
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        var command = new DeleteCurrencyCommand(ObjectId.Parse(id));
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var query = new GetCurrencyByIdQuery(ObjectId.Parse(id));
        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAllCurrenciesQuery(), cancellationToken);
        return Ok(response);
    }
}