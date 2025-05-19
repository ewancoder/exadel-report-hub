using ExportPro.Export.CQRS.Queries;
using FluentValidation;

namespace ExportPro.Export.Validations.Validations;

public sealed class DownloadLogByDateRangeQueryValidator : AbstractValidator<DownloadLogByDateRangeQuery>
{
    public DownloadLogByDateRangeQueryValidator()
    {
        RuleFor(x => x.startDate).NotEmpty().WithMessage("Start date is required.");
        RuleFor(x => x.endDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.startDate)
            .WithMessage("End date must be greater than or equal to start date.");
        RuleFor(x => x)
            .Must(
                (x) =>
                {
                    var daysBetween = x.endDate.DayNumber - x.startDate.DayNumber;
                    return daysBetween <= 30;
                }
            )
            .WithMessage("The date range must not exceed 30 days.");
    }
}
