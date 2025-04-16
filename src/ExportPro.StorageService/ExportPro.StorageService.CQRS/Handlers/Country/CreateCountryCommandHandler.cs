using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Country;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.Country;

public class CreateCountryCommandHandler(ICountryRepository repository) : ICommandHandler<CreateCountryCommand, Models.Models.Country>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<Models.Models.Country>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = new Models.Models.Country
        {
            Name = request.Name,
            Code = request.Code
        };

        await _repository.AddOneAsync(country, cancellationToken);
        return new BaseResponse<Models.Models.Country> { Data = country };
    }
}