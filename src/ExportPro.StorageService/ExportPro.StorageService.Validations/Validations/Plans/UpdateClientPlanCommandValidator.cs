using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public class UpdateClientPlanCommandValidator : AbstractValidator<UpdateClientPlanCommand>
{
    public UpdateClientPlanCommandValidator(IClientRepository clientRepository)
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
                            var plansResponse = await clientRepository.GetPlan(plan.ToObjectId(), cancellationToken);
                            return plansResponse != null;
                        }
                    )
                    .WithMessage("The Plan id does not exist in the client");
            })
            .DependentRules(() =>
            {
                RuleFor(x => x.PlansDto.StartDate)
                    .NotEmpty()
                    .WithMessage("Start date cannot be empty")
                    .GreaterThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("The start date must be greater than or equal to Today");
                RuleFor(x => x.PlansDto.EndDate)
                    .NotEmpty()
                    .WithMessage("End date cannot be empty")
                    .GreaterThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("The end date must be greater than or equal to Today");
                RuleFor(x => x.PlansDto)
                    .Must(x =>
                    {
                        var res = x.StartDate.CompareTo(x.EndDate);
                        if (res >= 0)
                            return false;
                        return true;
                    })
                    .WithMessage("The end date must be greater than start date");
            });
    }
}
