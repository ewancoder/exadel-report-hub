using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class SoftDeleteClientCommandValidator : AbstractValidator<SoftDeleteClientCommand>
{
    public SoftDeleteClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientId)
                    .MustAsync(
                        async (id, cancellationtoken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == id && !x.IsDeleted,
                                cancellationtoken
                            );
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            });
    }
}
