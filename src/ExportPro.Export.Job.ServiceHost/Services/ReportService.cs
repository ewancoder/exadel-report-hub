using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.Export.Job.ServiceHost.Services;

public class ReportService : IReportService
{
    public async Task SendScheduledReportsAsync()
    {
        var now = DateTime.UtcNow;
        var preferences = await _repository.GetEnabledPreferencesAsync();

        foreach (var pref in preferences)
        {
            // Skip if not the right time/day
            if (!ShouldSend(pref, now)) continue;

            // Build and send the report
            //await _reportGenerator.SendReportAsync(pref);
        }
    }

    private bool ShouldSend(ReportPreference pref, DateTime now)
    {
        if (!pref.IsEnabled) return false;

        // Only send at the right hour + minute
        if (pref.SendTime.Hour != now.Hour || pref.SendTime.Minute != now.Minute)
            return false;

        return pref.Frequency switch
        {
            ReportFrequency.Daily => true,
            ReportFrequency.Weekly => pref.DayOfWeek == now.DayOfWeek,
            ReportFrequency.Monthly => pref.DayOfMonth == now.Day,
            _ => false
        };
    }
}