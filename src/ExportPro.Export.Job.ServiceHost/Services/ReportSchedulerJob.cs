using ExportPro.Auth.SDK.DTOs;
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
    IConfiguration configuration
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var preferences = await reportRepository.GetAllPreferences(context.CancellationToken);
        var baseurl = Environment.GetEnvironmentVariable("DockerForAuth") ?? configuration["AuthURI"];
        HttpClient client = new() { BaseAddress = new Uri(baseurl) };

        IAuth authAPi = RestService.For<IAuth>(client);
        UserRegisterDto user = new()
        {
            Email = "G10@gmail.com",
            Username = "GPPP",
            Password = "G10@gmail.com",
        };

        UserLoginDto login = new() { Email = user.Email, Password = user.Password };
        var jwtTokenDto = await authAPi.LoginAsync(login);
        var jwtToken = jwtTokenDto.AccessToken;
        foreach (var pref in preferences)
        {
            if (!IsTimeToSend(pref))
                continue;
            var baseUrlForexport =
                Environment.GetEnvironmentVariable("DockerForReport") ?? configuration["ExportReportURI"];
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(baseUrlForexport), //localhost:5294"),
            };

            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
            IReportExportApi reportExportApi = RestService.For<IReportExportApi>(httpClient);
            var reportResponse = await reportExportApi.GetStatisticsAsync(
                pref.ReportFormat,
                pref.ClientId.ToGuid(),
                context.CancellationToken
            );

            if (!reportResponse.IsSuccessStatusCode)
                continue;

            var (extension, mimeType) = pref.ReportFormat switch
            {
                ReportFormat.Xlsx => ("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                ReportFormat.Csv => ("csv", "text/csv"),
                _ => ("dat", "application/octet-stream"), // fallback
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

        var cron = new CronExpression(pref.CronExpression) { TimeZone = TimeZoneInfo.Utc };

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
