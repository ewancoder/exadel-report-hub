using ExportPro.Export.Job.ServiceHost.DTOs;
using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.Export.Job.ServiceHost.Services;

namespace ExportPro.Export.Job.ServiceHost.Configurations;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var smtpSettings = config.GetSection("SmtpSettings").Get<SmtpSettings>();
        services.AddSingleton(smtpSettings);
        services.AddSingleton<IReportGeneratorFactory, ReportGeneratorFactory>();

        services.AddTransient<IEmailSender, SmtpEmailSender>();
        services.AddTransient<IInvoiceReportService, InvoiceReportService>();
    }
}