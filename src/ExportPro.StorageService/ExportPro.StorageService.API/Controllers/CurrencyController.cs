using ExportPro.StorageService.CQRS.Commands.CurrencyCommand;
using ExportPro.StorageService.CQRS.Queries.CurrencyQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid currency ID format.");

        command.Id = objectId;
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid currency ID format.");

        var response = await _mediator.Send(new DeleteCurrencyCommand { Id = objectId }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Invalid currency ID format.");

        var response = await _mediator.Send(new GetCurrencyByIdQuery { Id = objectId }, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllCurrenciesQuery(), cancellationToken);
        return Ok(response);
    }
}