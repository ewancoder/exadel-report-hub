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
    public async Task<IActionResult> Create(
        [FromBody] CreateCurrencyCommand command,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] string CurrencyCode,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new UpdateCurrencyCommand(id, CurrencyCode));
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteCurrencyCommand(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCurrencyByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAllCurrenciesQuery(), cancellationToken);
        return Ok(response);
    }
}
