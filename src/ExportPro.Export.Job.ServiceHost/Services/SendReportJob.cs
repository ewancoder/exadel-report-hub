using ExportPro.Export.Job.ServiceHost.Interfaces;
using Quartz;

namespace ExportPro.Export.Job.ServiceHost.Services;

public sealed class SendReportJob(IReportService reportService) : IJob
{
    private readonly IReportService _reportService = reportService;

    public async Task Execute(IJobExecutionContext context)
    {
        // The job will scan preferences and send emails as needed
        await _reportService.SendScheduledReportsAsync();
    }
}