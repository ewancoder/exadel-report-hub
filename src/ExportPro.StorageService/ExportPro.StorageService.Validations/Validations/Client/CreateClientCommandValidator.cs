using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportPro.StorageService.CQRS.CommandHandlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Plans;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Clientdto.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters")
            .DependentRules(() =>
            {
                RuleFor(x => x.Clientdto.Name)
                    .MustAsync(
                        async (Name, _) =>
                        {
                            var client = await clientRepository.ClientExists(Name);
                            return !client;
                        }
                    )
                    .WithMessage("Client with this name already exists");
            });
    }
}
