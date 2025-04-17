using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class GetClientByIdValidator:AbstractValidator<GetClientByIdQuery>
{
    public GetClientByIdValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Id).SetValidator(new ClientIdValidator(clientRepository));
    }
}
