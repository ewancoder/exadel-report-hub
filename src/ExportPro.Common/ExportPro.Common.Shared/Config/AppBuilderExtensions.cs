using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ExportPro.Common.Shared.Config;

public static class AppBuilderExtensions
{
    public static IHostBuilder UseSharedSerilogAndConfiguration(this IHostBuilder builder)
    {
        builder
            .UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration);
            })
            .ConfigureAppConfiguration((hostContext, configBuilder) =>
            {
                var env = hostContext.HostingEnvironment;
                configBuilder.AddJsonFile($"{env.ContentRootPath}\\Settings\\serilogsettings.json");
                configBuilder.AddEnvironmentVariables();
            });

        return builder;
    }
}

