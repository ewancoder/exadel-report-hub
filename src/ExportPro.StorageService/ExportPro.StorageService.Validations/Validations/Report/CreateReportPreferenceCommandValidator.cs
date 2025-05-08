using ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Report;

public class CreateReportPreferenceCommandValidator : AbstractValidator<CreateReportPreferenceCommand>
{
    public CreateReportPreferenceCommandValidator()
    {
        RuleFor(x => x.dto.ReportFormat)
           .NotEmpty().WithMessage("ClientId is required.");

        //RuleFor(x => x.dto.ReportFormat)
        //    .NotNull().WithMessage("Schedule is required.")
        //    .SetValidator(new ReportScheduleDtoValidator());
        RuleFor(x => x.dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");
    }
}