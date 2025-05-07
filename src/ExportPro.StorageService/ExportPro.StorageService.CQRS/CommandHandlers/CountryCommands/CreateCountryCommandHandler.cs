using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public sealed record CreateCountryCommand(CreateCountryDto CountryDto) : ICommand<CountryDto>;

public sealed class CreateCountryCommandHandler(
    IHttpContextAccessor httpContext,
    ICountryRepository repository,
    IMapper mapper
) : ICommandHandler<CreateCountryCommand, CountryDto>
{
    public async Task<BaseResponse<CountryDto>> Handle(
        CreateCountryCommand request,
        CancellationToken cancellationToken
    )
    {
        var country = new Country
        {
            Name = request.CountryDto.Name,
            Code = request.CountryDto.Code,
            CurrencyId = request.CountryDto.CurrencyId.ToObjectId(),
            CreatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value,
        };
        await repository.AddOneAsync(country, cancellationToken);
        return new SuccessResponse<CountryDto>(mapper.Map<CountryDto>(country));
    }
}
