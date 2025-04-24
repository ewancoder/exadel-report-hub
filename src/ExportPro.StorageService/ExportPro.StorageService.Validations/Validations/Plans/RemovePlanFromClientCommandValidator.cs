using ExportPro.StorageService.CQRS.CommandHandlers.Plans;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public sealed class RemovePlanFromClientCommandValidator : AbstractValidator<RemovePlanFromClientCommand>
{
    public RemovePlanFromClientCommandValidator(IClientRepository clientRepository)
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
                        async (id, cancellationToken) =>
                        {
                            var client = await clientRepository.GetByIdAsync(ObjectId.Parse(id), cancellationToken);
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            })
            .DependentRules(
                () =>
                    RuleFor(x => x)
                        .MustAsync(
                            async (plan, cancellationToken) =>
                            {
                                var client = await clientRepository.GetByIdAsync(
                                    ObjectId.Parse(plan.clientId),
                                    cancellationToken
                                );
                                if (client.Plans.Any(x => x.Id.ToString() == plan.planId && !x.IsDeleted))
                                    return true;
                                return false;
                            }
                        )
                        .WithMessage("The Plan id does not exist in the client")
            );
    }
}
