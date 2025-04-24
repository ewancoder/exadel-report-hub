using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator(IClientRepository clientRepository)
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
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == ObjectId.Parse(id) && !x.IsDeleted,
                                cancellationToken
                            );
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            })
            .DependentRules(() =>
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
                                async (Name, cancellationToken) =>
                                {
                                    var client = await clientRepository.GetOneAsync(
                                        x => x.Name == Name && !x.IsDeleted,
                                        cancellationToken
                                    );
                                    return client == null;
                                }
                            )
                            .WithMessage("Client with this name already exists");
                    });
            });
    }
}
