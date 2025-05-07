using ExportPro.Common.Shared.Extensions;
using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using Quartz;

namespace ExportPro.Export.Job.ServiceHost.Services;

public sealed class ReportSchedulerJob(
    IReportPreference repository,
    IReportExportApi reportExportApi,
    IEmailService emailService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var preferences = await repository.GetAllPreferences(context.CancellationToken);

        foreach (var pref in preferences)
        {
            if (!IsTimeToSend(pref))
                continue;

            var reportResponse = await reportExportApi.GetStatisticsAsync(
                pref.ReportFormat,
                pref.ClientId.ToGuid(),
                context.CancellationToken);

            if (!reportResponse.IsSuccessStatusCode)
                continue;

            var content = await reportResponse.Content.ReadAsByteArrayAsync();
            var contentType = reportResponse.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = $"report_{DateTime.UtcNow:yyyyMMddHHmm}.csv";

            var subject = "Scheduled Report";
            var body = $"Dear user,\n\nPlease find your scheduled report attached.";

            var userEmail = pref.Email; // Replace 

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                await emailService.SendAsync(userEmail, subject, body, content, fileName, contentType);
            }
        }
    }

    private static bool IsTimeToSend(ReportPreference pref)
    {
        if (string.IsNullOrWhiteSpace(pref.CronExpression))
            return false;

        var cron = new CronExpression(pref.CronExpression)
        {
            TimeZone = TimeZoneInfo.Utc
        };

        var now = DateTime.UtcNow;
        return cron.IsSatisfiedBy(now);
    }
}