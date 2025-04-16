using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Clientdto.Name)
            .NotEmpty()
            .MinimumLength(5)
            .WithMessage("Name must be higher than 5 characters")
            .MaximumLength(50)
            .WithMessage("Name lower then 50 characters")
            .MustAsync(
                async (name, _) =>
                {
                    var exists = await clientRepository.ClientExists(name);
                    return !exists;
                }
            )
            .WithMessage("Client exists.Please enter diffrent  client name");
    }
}
