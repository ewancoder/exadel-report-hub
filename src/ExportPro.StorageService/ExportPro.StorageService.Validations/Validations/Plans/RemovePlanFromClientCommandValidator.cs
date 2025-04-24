using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public sealed class RemovePlanFromClientCommandValidator : AbstractValidator<RemovePlanFromClientCommand>
{
    public RemovePlanFromClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.PlanId)
            .NotEmpty()
            .WithMessage("Plan  Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Plan Id is not valid in format.")
            .MustAsync(
                async (plan, cancellationToken) =>
                {
                    var plansResponse = await clientRepository.GetPlan(plan, cancellationToken);
                    return plansResponse != null;
                }
            )
            .WithMessage("The Plan id does not exist in the client");
    }
}
