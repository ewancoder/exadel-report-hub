using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Country;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.Country;

public class CreateCountryCommandHandler : ICommandHandler<CreateCountryCommand, Models.Models.Country>
{
    private readonly IRepository<Models.Models.Country> _repository;

    public CreateCountryCommandHandler(IRepository<Models.Models.Country> repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<Models.Models.Country>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = new Models.Models.Country
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Code = request.Code
        };

        await _repository.AddOneAsync(country, cancellationToken);

        return new BaseResponse<Models.Models.Country>
        {
            Data = country,
            ApiState = HttpStatusCode.Created,
            IsSuccess = true
        };
    }
}