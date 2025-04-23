using ExportPro.StorageService.CQRS.CommandHandlers.Plans;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public class UpdateClientPlanCommandValidator : AbstractValidator<UpdateClientPlanCommand>
{
    public UpdateClientPlanCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.clientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Client Id is not valid in format.")
            .DependentRules(() =>
            {
                RuleFor(x => x.clientId)
                    .MustAsync(
                        async (id, _) =>
                        {
                            var client = await clientRepository.GetClientById(id);
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            })
            .DependentRules(() =>
            {
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
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.plansDto.StartDate)
                            .NotEmpty()
                            .WithMessage("Start date cannot be empty")
                            .GreaterThanOrEqualTo(DateTime.UtcNow)
                            .WithMessage("The start date must be greater than or equal to Today");
                        RuleFor(x => x.plansDto.EndDate)
                            .NotEmpty()
                            .WithMessage("End date cannot be empty")
                            .GreaterThanOrEqualTo(DateTime.UtcNow)
                            .WithMessage("The end date must be greater than or equal to Today");
                        RuleFor(x => x.plansDto)
                            .Must(x =>
                            {
                                var res = x.StartDate.CompareTo(x.EndDate);
                                if (res >= 0)
                                    return false;
                                return true;
                            })
                            .WithMessage("The end date must be greater than start date");
                    });
            });
    }
}
