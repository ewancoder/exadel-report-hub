using ExportPro.StorageService.CQRS.CommandHandlers.PlanCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public sealed class AddPlanToClientCommandValidator : AbstractValidator<AddPlanToClientCommand>
{
    public AddPlanToClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientId)
                    .MustAsync(
                        async (id, cancellationToken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == id.ToObjectId() && !x.IsDeleted,
                                cancellationToken
                            );
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            })
            .DependentRules(() =>
            {
                RuleFor(x => x.Plan.StartDate)
                    .NotEmpty()
                    .WithMessage("Start date cannot be empty")
                    .GreaterThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("The start date must be greater than or equal to Today");
                RuleFor(x => x.Plan.EndDate)
                    .NotEmpty()
                    .WithMessage("End date cannot be empty")
                    .GreaterThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("The end date must be greater than or equal to Today");
                RuleFor(x => x.Plan)
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
