using ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Report;

public class CreateReportPreferenceCommandValidator : AbstractValidator<CreateReportPreferenceCommand>
{
    public CreateReportPreferenceCommandValidator()
    {
        RuleFor(x => x.ClientId)
           .NotEmpty().WithMessage("ClientId is required.");

        RuleFor(x => x.Schedule)
            .NotNull().WithMessage("Schedule is required.")
            .SetValidator(new ReportScheduleDtoValidator());
    }
}