using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public sealed record CreateCountryCommand(CreateCountryDto CountryDto) : ICommand<CountryDto>;

public sealed class CreateCountryCommandHandler(ICountryRepository repository, IMapper mapper)
    : ICommandHandler<CreateCountryCommand, CountryDto>
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
        };
        await repository.AddOneAsync(country, cancellationToken);
        return new SuccessResponse<CountryDto>(mapper.Map<CountryDto>(country));
    }
}
