using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public sealed class RemovePlanFromClientCommandValidator : AbstractValidator<RemovePlanFromClientCommand>
{
    public RemovePlanFromClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.PlanId)
            .NotEmpty()
            .WithMessage("Plan  Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.PlanId)
                    .MustAsync(
                        async (plan, cancellationToken) =>
                        {
                            var plansResponse = await clientRepository.GetPlan(plan, cancellationToken);
                            return plansResponse != null;
                        }
                    )
                    .WithMessage("The Plan id does not exist in the client");
            });
    }
}
