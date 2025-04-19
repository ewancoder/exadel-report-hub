using ExportPro.StorageService.CQRS.Handlers.Plans;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public sealed class RemovePlanFromClientCommandValidator : AbstractValidator<RemovePlanFromClientCommand>
{
    public RemovePlanFromClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.clientId)
            .SetValidator(new ClientIdValidator(clientRepository))
            .DependentRules(
                () =>
                    RuleFor(x => x)
                        .MustAsync(
                            async (plan, _) =>
                            {
                                var client = await clientRepository.GetClientById(plan.clientId);
                                if (client.Plans.Any(x => x.Id.ToString() == plan.planId && !x.IsDeleted))
                                    return true;
                                return false;
                            }
                        )
                        .WithMessage("The Plan id does not exist in the client")
            );
    }
}
