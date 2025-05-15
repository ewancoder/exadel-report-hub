using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.Export.Job.Utilities.Helpers;

public static class CronHelper
{
    public static string ToCron(ReportScheduleDto schedule)
    {
        var hour = schedule.Time.Hour;
        var minute = schedule.Time.Minute;

        return schedule.Frequency switch
        {
            ReportFrequency.Daily => $"0 {minute} {hour} ? * *",

            ReportFrequency.Weekly when schedule.DayOfWeek.HasValue =>
                $"0 {minute} {hour} ? * {(int)schedule.DayOfWeek.Value + 1}",

            ReportFrequency.Monthly when schedule.DayOfMonth.HasValue =>
                $"0 {minute} {hour} {schedule.DayOfMonth.Value} * ?",

            _ => throw new InvalidOperationException("Invalid schedule configuration"),
        };
    }
}
