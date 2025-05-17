using ExportPro.Export.Job.ServiceHost.Services;
using Quartz;

namespace ExportPro.Export.Job.ServiceHost.Configurations;

public static class ScheduleConfig
{
    public static void AddScheduleService(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(ReportSchedulerJob));

            q.AddJob<ReportSchedulerJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(ReportSchedulerJob)}-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
            );
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
