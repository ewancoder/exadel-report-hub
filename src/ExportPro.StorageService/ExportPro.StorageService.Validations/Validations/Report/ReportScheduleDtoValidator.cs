using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.SDK.DTOs;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Report;

public class ReportScheduleDtoValidator : AbstractValidator<ReportScheduleDto>
{
    public ReportScheduleDtoValidator()
    {
        RuleFor(x => x.Time)
            .NotNull().WithMessage("Time is required.")
            .Must(BeValidTimeOnly).WithMessage("Time must be a valid time in 24-hour format (00:00 to 23:59).");

        RuleFor(x => x.Frequency)
            .IsInEnum().WithMessage("Frequency is not valid.");

        RuleFor(x => x)
            .Custom((schedule, context) =>
            {
                switch (schedule.Frequency)
                {
                    case ReportFrequency.Weekly:
                        if (schedule.DayOfWeek is null)
                        {
                            context.AddFailure("DayOfWeek must be set for weekly frequency.");
                        }
                        else if ((int)schedule.DayOfWeek < 0 || (int)schedule.DayOfWeek > 6)
                        {
                            context.AddFailure("DayOfWeek must be between 0 (Sunday) and 6 (Saturday).");
                        }
                        break;

                    case ReportFrequency.Monthly:
                        if (schedule.DayOfMonth is null)
                        {
                            context.AddFailure("DayOfMonth must be set for monthly frequency.");
                        }
                        else if (schedule.DayOfMonth < 1 || schedule.DayOfMonth > 31)
                        {
                            context.AddFailure("DayOfMonth must be between 1 and 31.");
                        }
                        break;

                    case ReportFrequency.Daily:
                        if (schedule.DayOfWeek != null || schedule.DayOfMonth != null)
                        {
                            context.AddFailure("DayOfWeek and DayOfMonth must be null for daily frequency.");
                        }
                        break;
                }
            });
    }

    private bool BeValidTimeOnly(TimeOnly time) =>
        time.Hour >= 0 && time.Hour < 24 &&
        time.Minute >= 0 && time.Minute < 60;
}