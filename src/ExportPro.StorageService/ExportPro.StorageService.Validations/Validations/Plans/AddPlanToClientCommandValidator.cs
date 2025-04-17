using ExportPro.StorageService.CQRS.Handlers.Plans;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public sealed class AddPlanToClientCommandValidator : AbstractValidator<AddPlanToClientCommand>
{
    public AddPlanToClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.clientId)
            .SetValidator(new ClientIdValidator(clientRepository))
            .DependentRules(() =>
            {
                RuleFor(x => x.plan).SetValidator(new PlansValidator());
            });
    }
}
