using ExportPro.AuthService.Seeder;

namespace ExportPro.Auth.ServiceHost.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedDataAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting data seeding process...");

            var userSeeder = services.GetRequiredService<UserSeeder>();
            await userSeeder.SeedAsync();

            logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public static IServiceCollection AddDataSeeders(this IServiceCollection services)
    {
        services.AddScoped<UserSeeder>();

        return services;
    }
}
