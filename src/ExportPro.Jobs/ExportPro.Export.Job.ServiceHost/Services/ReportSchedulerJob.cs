using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Export.Job.ServiceHost.DTOs;
using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using Microsoft.Extensions.Options;
using Quartz;
using Refit;
using ILogger = Serilog.ILogger;

namespace ExportPro.Export.Job.ServiceHost.Services;

public sealed class ReportSchedulerJob(
    IReportPreference reportRepository,
    IEmailService emailService,
    ILogger logger,
    IConfiguration configuration,
    IOptions<ServiceAccountSettings> serviceAccountOptions
) : IJob
{
    private readonly ServiceAccountSettings _serviceAccount = serviceAccountOptions.Value;
    public async Task Execute(IJobExecutionContext context)
    {
        var preferences = await reportRepository.GetAllPreferences(context.CancellationToken);
        logger.Debug("starting job");
        var baseurl = Environment.GetEnvironmentVariable("DockerForAuth") ?? configuration["AuthURI"];
        logger.Debug("docker auth uri: {0}", baseurl);
        HttpClient client = new() { BaseAddress = new Uri(baseurl!) };

        IAuth authAPi = RestService.For<IAuth>(client);
        var login = new UserLoginDto
        {
            Email = _serviceAccount.Email,
            Password = _serviceAccount.Password
        };

        var jwtTokenDto = await authAPi.LoginAsync(login);
        logger.Debug("jwt token: {0}", jwtTokenDto.Data.AccessToken);
        var jwtToken = jwtTokenDto.Data.AccessToken;
            var baseUrlForexport =
                Environment.GetEnvironmentVariable("DockerForReport") ?? configuration["ExportReportURI"];
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(baseUrlForexport!),
            };

            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
            IReportExportApi reportExportApi = RestService.For<IReportExportApi>(httpClient);

        foreach (var pref in preferences)
        {
            if (!IsTimeToSend(pref))
                continue;
    logger.Debug("sending preferences for {0}", pref);
    logger.Debug("email: {0}", pref.Email);
            var reportResponse = await reportExportApi.GetStatisticsAsync(
                                pref.ReportFormat,
                                pref.ClientId.ToGuid(),
                                pref.ClientCurrencyId.ToGuid(),
                                context.CancellationToken
                            );

            if (!reportResponse.IsSuccessStatusCode)
                continue;

            var (extension, mimeType) = pref.ReportFormat switch
            {
                ReportFormat.Xlsx => ("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                ReportFormat.Csv => ("csv", "text/csv"),
                _ => ("dat", "application/octet-stream"),
            };
            var httpResponse = reportResponse.Content;
            var fileName = $"report_{DateTime.UtcNow:yyyyMMddHHmm}.{extension}";
            var content = await httpResponse.Content.ReadAsByteArrayAsync(context.CancellationToken);
            var contentType = httpResponse.Content.Headers.ContentType?.MediaType ?? mimeType;
            var subject = $"Scheduled Report - {DateTime.UtcNow:MMMM dd, yyyy}";
            var body = $"Dear user,\n\nPlease find your scheduled report attached.";
            var userEmail = pref.Email;

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                await emailService.SendAsync(new EmailSendDto
                {
                    To = userEmail,
                    Subject = subject,
                    Body = body,
                    Attachment = content,
                    FileName = fileName,
                    ContentType = contentType
                });
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
