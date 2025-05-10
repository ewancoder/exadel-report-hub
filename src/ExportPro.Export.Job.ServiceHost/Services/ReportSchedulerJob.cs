using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using Quartz;
using Refit;

namespace ExportPro.Export.Job.ServiceHost.Services;

public sealed class ReportSchedulerJob(
    IReportPreference reportRepository,
    IEmailService emailService,
    IHttpContextAccessor httpContext) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var authHeader = httpContext.HttpContext?.Request.Headers["Authorization"].ToString();
        var preferences = await reportRepository.GetAllPreferences(context.CancellationToken);

        foreach (var pref in preferences)
        {
            if (!IsTimeToSend(pref))
                continue;
            HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri("https://localhost:7195");
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", pref.JwtToken);
            IReportExportApi reportExportApi = RestService.For<IReportExportApi>(httpClient);
            var reportResponse = await reportExportApi.GetStatisticsAsync(
                pref.ReportFormat,
                pref.ClientId.ToGuid(),
                context.CancellationToken);

            if (!reportResponse.IsSuccessStatusCode)
                continue;

            var (extension, mimeType) = pref.ReportFormat switch
            {
                ReportFormat.Xlsx => ("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                ReportFormat.Csv => ("csv", "text/csv"),
                _ => ("dat", "application/octet-stream") // fallback
            };

            var fileName = $"report_{DateTime.UtcNow:yyyyMMddHHmm}.{extension}";
            var contentType = reportResponse.Content.Headers.ContentType?.MediaType ?? mimeType;
            var content = await reportResponse.Content.ReadAsByteArrayAsync(context.CancellationToken);
            var subject = $"Scheduled Report - {DateTime.UtcNow:MMMM dd, yyyy}";
            var body = $"Dear user,\n\nPlease find your scheduled report attached.";
            var userEmail = pref.Email;

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
        var previous = cron.GetTimeBefore(now);
        var next = cron.GetNextValidTimeAfter(previous ?? now.AddMinutes(-1)); // fallback if previous is null

        // We want to run the job once per scheduled minute, at any second
        if (!next.HasValue)
            return false;

        var nextFire = next.Value.UtcDateTime;

        // Allow execution if now is within the current fire minute
        return now >= nextFire && now < nextFire.AddMinutes(1);
    }
}