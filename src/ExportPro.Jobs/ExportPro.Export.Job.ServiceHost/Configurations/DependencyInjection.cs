using ExportPro.Export.Job.ServiceHost.DTOs;
using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.Export.Job.ServiceHost.Services;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.SDK.Refit;
using Refit;

namespace ExportPro.Export.Job.ServiceHost.Configurations;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var smtpSettings = config.GetSection("SmtpSettings").Get<SmtpSettings>();
        services.AddSingleton(smtpSettings);
        var baseurl = Environment.GetEnvironmentVariable("DockerForReport") ?? config["ReportExportApi:BaseUrl"];
        services.AddRefitClient<IReportExportApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(baseurl!));

        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<IReportPreference, ReportPreferenceRepository>();
        services.AddTransient<ReportSchedulerJob>();
        Console.WriteLine("Quartz job registration completed");
    }
}
