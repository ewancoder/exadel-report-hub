using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.CountryQueries;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public Task<BaseResponse<CountryDto>> Create(
        [FromBody] CreateCountryDto country,
        CancellationToken cancellationToken
    ) => mediator.Send(new CreateCountryCommand(country), cancellationToken);

    [HttpPut("{id}")]
    public Task<BaseResponse<bool>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCountry country,
        CancellationToken cancellationToken
    ) => mediator.Send(new UpdateCountryCommand(id, country), cancellationToken);

    [HttpDelete("{id}")]
    public Task<BaseResponse<bool>> Delete([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new DeleteCountryCommand(id), cancellationToken);

    [HttpGet("{id}")]
    public Task<BaseResponse<CountryDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetCountryByIdQuery(id), cancellationToken);

    [HttpGet]
    public Task<BaseResponse<PaginatedListDto<CountryDto>>> GetAll(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    ) =>
        mediator.Send(
            new GetPaginatedCountriesQuery { PageNumber = pageNumber, PageSize = pageSize },
            cancellationToken
        );
}
