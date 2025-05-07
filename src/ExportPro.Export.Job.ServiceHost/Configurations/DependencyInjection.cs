using ExportPro.Export.Job.ServiceHost.DTOs;
using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.Export.Job.ServiceHost.Services;
using ExportPro.StorageService.SDK.Refit;
using Refit;

namespace ExportPro.Export.Job.ServiceHost.Configurations;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var smtpSettings = config.GetSection("SmtpSettings").Get<SmtpSettings>();
        services.AddSingleton(smtpSettings);
        services.AddRefitClient<IReportExportApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://your-api-base-url"));

        services.AddTransient<IEmailService, EmailService>();
    }
}