using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ExportPro.Common.Shared.Config;

public static class AppBuilderExtensions
{
    public static IHostBuilder UseSharedSerilogAndConfiguration(this IHostBuilder builder)
    {
        builder
            .UseSerilog(
                (ctx, lc) =>
                {
                    lc.ReadFrom.Configuration(ctx.Configuration);
                }
            )
            .ConfigureAppConfiguration(
                (hostContext, configBuilder) =>
                {
                    var env = hostContext.HostingEnvironment;

                    var relativePath =Path.Combine("Settings", "serilogsettings.json");

                    var fullPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, relativePath));

                    Console.WriteLine(
                        "CONFIG DEBUG: Loading Serilog settings from: "
                            + Path.Combine(env.ContentRootPath, "Settings", "serilogsettings.json")
                    );

                    Console.WriteLine(
                        "Does config file exist? "
                            + File.Exists(Path.Combine(env.ContentRootPath, "Settings", "serilogsettings.json"))
                    );

                    configBuilder.AddJsonFile(fullPath, optional: false, reloadOnChange: true);
                    configBuilder.AddEnvironmentVariables();
                }
            );
        return builder;
    }
}
