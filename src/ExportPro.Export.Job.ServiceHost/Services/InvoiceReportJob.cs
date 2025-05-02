using ExportPro.Export.Job.ServiceHost.Interfaces;
using Quartz;

namespace ExportPro.Export.Job.ServiceHost.Services;

public class InvoiceReportJob(
    IInvoiceReportService reportService, 
    IReportGeneratorFactory factory, 
    IEmailSender emailSender) 
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var reportContent = await reportService.BuildReportContentAsync();
            var generator = factory.GetGenerator("xlsx");
            var bytes = generator.Generate(reportContent);

            await emailSender.SendAsync("user@domain.com", "Scheduled Report", "Attached is your report.", bytes, $"report.{generator.Extension}", generator.ContentType);
        }
        catch (Exception ex)
        {
            await emailSender.SendAsync("admin@domain.com", "Report Failed", ex.Message);
        }
    }
}