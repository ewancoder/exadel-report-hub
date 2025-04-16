using ExportPro.StorageService.CQRS.Handlers.Client;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations;

public sealed  class UpdateClientCommandValidator:AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {

    }
}
