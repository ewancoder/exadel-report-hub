using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public sealed class CreateCountryCommand : ICommand<CountryDto>
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public Guid CurrencyId { get; set; }
}
public sealed class CreateCountryCommandHandler(
    ICountryRepository repository, 
    IMapper mapper, 
    IValidator<CreateCountryCommand> validator)
    : ICommandHandler<CreateCountryCommand, CountryDto>
{
    public async Task<BaseResponse<CountryDto>> Handle(
        CreateCountryCommand request,
        CancellationToken cancellationToken
    )
    {
        var validate = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validate.IsValid)
        {
            return new BaseResponse<CountryDto>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Country name is required."],
            };
        }
        var country = new Country
        {
            Name = request.Name,
            Code = request?.Code,
            CurrencyId = request.CurrencyId.ToObjectId(),
        };
        await repository.AddOneAsync(country, cancellationToken);
        return new BaseResponse<CountryDto>
        {
            IsSuccess = true,
            ApiState = HttpStatusCode.Created,
            Data = mapper.Map<CountryDto>(country),
        };
    }
}