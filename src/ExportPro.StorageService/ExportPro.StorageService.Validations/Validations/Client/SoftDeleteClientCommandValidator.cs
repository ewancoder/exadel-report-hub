using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class SoftDeleteClientCommandValidator:AbstractValidator<SoftDeleteClientCommand>
{
    public SoftDeleteClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x=>x.ClientId).SetValidator(new ClientIdValidator(clientRepository));
    }
}