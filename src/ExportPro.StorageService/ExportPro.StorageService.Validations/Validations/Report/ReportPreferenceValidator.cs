using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Report;

public class ReportPreferenceValidator : AbstractValidator<ReportPreference>
{
    public ReportPreferenceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.SendTime)
            .NotNull().WithMessage("TimeToSend is required.");
        
        RuleFor(x => x.SendTime)
            .NotEmpty().WithMessage("SendTime is required.");

        RuleFor(x => x.SendTime)
            .Must(BeValidTimeOnly).WithMessage("SendTime must be a valid time in 24-hour format (00:00 to 23:59).");

        RuleFor(x => x.Frequency)
            .IsInEnum().WithMessage("Frequency is not valid.");

        RuleFor(x => x)
            .Custom((pref, context) =>
            {
                switch (pref.Frequency)
                {
                    case ReportFrequency.Weekly:
                        if (pref.DayOfWeek is null)
                            context.AddFailure("DayOfWeek must be set for weekly frequency.");
                        break;

                    case ReportFrequency.Monthly:
                        if (pref.DayOfMonth is null)
                            context.AddFailure("DayOfMonth must be set for monthly frequency.");
                        else if (pref.DayOfMonth < 1 || pref.DayOfMonth > 31)
                            context.AddFailure("DayOfMonth must be between 1 and 31.");
                        break;

                    case ReportFrequency.Daily:
                        if (pref.DayOfWeek is not null || pref.DayOfMonth is not null)
                            context.AddFailure("DayOfWeek and DayOfMonth must be null for daily frequency.");
                        break;
                }
            });
    }
    private bool BeValidTimeOnly(TimeOnly time) =>
        time.Hour >= 0 && time.Hour < 24 &&
        time.Minute >= 0 && time.Minute < 60;
}