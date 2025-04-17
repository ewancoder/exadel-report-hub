using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.ClientId).SetValidator(new ClientIdValidator(clientRepository))
            .DependentRules(() =>
            {
                RuleFor(x => x.clientUpdateDto.Name).SetValidator(new ClientNameValidator(clientRepository));
            });
    }
}
