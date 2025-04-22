using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CountryCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class CreateCountryCommandHandler(ICountryRepository repository, IMapper mapper)
    : ICommandHandler<CreateCountryCommand, CountryDto>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<CountryDto>> Handle(
        CreateCountryCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new BaseResponse<CountryDto>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Country name is required."]
            };
        }

        var country = new Country
        {
            Name = request.Name,
            Code = request?.Code,
            CurrencyId = request?.CurrencyId,
        };

        await _repository.AddOneAsync(country, cancellationToken);

        return new BaseResponse<CountryDto>
        {
            IsSuccess = true,
            ApiState = HttpStatusCode.Created,
            Data = mapper.Map<CountryDto>(country),
        };
    }
}
