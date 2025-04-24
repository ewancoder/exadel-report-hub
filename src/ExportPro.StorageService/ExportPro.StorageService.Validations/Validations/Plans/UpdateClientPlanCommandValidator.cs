using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public class UpdateClientPlanCommandValidator : AbstractValidator<UpdateClientPlanCommand>
{
    public UpdateClientPlanCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x)
            .MustAsync(
                async (plan, cancellationToken) =>
                {
                    var client = await clientRepository.GetByIdAsync(ObjectId.Parse(plan.PlanId), cancellationToken);
                    if (client.Plans.Any(x => x.Id.ToString() == plan.PlanId && !x.IsDeleted))
                        return true;
                    return false;
                }
            )
            .WithMessage("The Plan id does not exist in the client")
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
