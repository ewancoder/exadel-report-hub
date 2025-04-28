using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public sealed class CreateCountryCommand : ICommand<CountryDto>
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? CurrencyId { get; set; }
}
public sealed class CreateCountryCommandHandler(
    ICountryRepository repository, 
    IMapper mapper, 
    IValidator<CreateCountryCommand> validator)
    : ICommandHandler<CreateCountryCommand, CountryDto>
{
    private readonly ICountryRepository _repository = repository;
    private readonly IValidator<CreateCountryCommand> _validator = validator;
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