using Microsoft.Extensions.Configuration;

namespace ExportPro.Shared.IntegrationTests.Configs;

public static class LoadingConfig
{
    public static IConfigurationRoot LoadConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json", false, true)
            .Build();
    }
}
