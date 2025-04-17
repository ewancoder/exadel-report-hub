using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CountryCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class CreateCountryCommandHandler(ICountryRepository repository) : ICommandHandler<CreateCountryCommand, Country>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<Country>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = new Country
        {
            Name = request.Name,
            Code = request.Code
        };

        await _repository.AddOneAsync(country, cancellationToken);
        return new BaseResponse<Country> { Data = country };
    }
}