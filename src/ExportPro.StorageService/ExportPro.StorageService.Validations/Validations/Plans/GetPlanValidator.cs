using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public class GetPlanValidator : AbstractValidator<GetPlanQuery>
{
    public GetPlanValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.PlanId)
            .NotEmpty()
            .WithMessage("Plan Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.PlanId)
                    .MustAsync(
                        async (id, cancellationToken) =>
                        {
                            var plan = await clientRepository.GetPlan(id.ToObjectId(), cancellationToken);
                            return plan != null;
                        }
                    )
                    .WithMessage("The Plan Id does not exist");
            });
    }
}
