﻿using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.QueryHandlers.CountryQueries;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Refit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController(IMediator mediator) : ControllerBase, ICountryController
{
    [HttpGet("name/{countryCode}")]
    public Task<BaseResponse<CountryDto>> GetByCode(
        [FromRoute] string countryCode,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new GetCountryByCodeQuery(countryCode), cancellationToken);
    }

    [HttpGet("{id}")]
    public Task<BaseResponse<CountryDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetCountryByIdQuery(id), cancellationToken);
    }

    [HttpGet]
    public Task<BaseResponse<PaginatedListDto<CountryDto>>> GetAll(
        [FromQuery] PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.Send(new GetPaginatedCountriesQuery(paginationParameters), cancellationToken);
    }
}
