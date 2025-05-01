using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters")
            .DependentRules(() =>
            {
                RuleFor(x => x.Name)
                    .MustAsync(
                        async (Name, cancellationtoken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Name == Name && !x.IsDeleted,
                                cancellationtoken
                            );
                            return client == null;
                        }
                    )
                    .WithMessage("Client with this name already exists");
            });
    }
}
