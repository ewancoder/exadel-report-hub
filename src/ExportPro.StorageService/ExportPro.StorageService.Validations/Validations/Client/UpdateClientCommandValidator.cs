using ExportPro.StorageService.CQRS.CommandHandlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Client Id is not valid in format.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientId)
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
                RuleFor(x => x.clientUpdateDto.Name)
                    .NotEmpty()
                    .WithMessage("Name must not be empty")
                    .MinimumLength(3)
                    .WithMessage("Name must be at least 3 characters long")
                    .MaximumLength(50)
                    .WithMessage("Name must not exceed 50 characters")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.clientUpdateDto.Name)
                            .MustAsync(
                                async (Name, _) =>
                                {
                                    var client = await clientRepository.ClientExists(Name);
                                    return !client;
                                }
                            )
                            .WithMessage("Client with this name already exists");
                    });
            });
    }
}
