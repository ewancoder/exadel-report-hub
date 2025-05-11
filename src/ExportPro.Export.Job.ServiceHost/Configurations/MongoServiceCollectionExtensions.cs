using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using MongoDB.Driver;

namespace ExportPro.Export.Job.ServiceHost.Configurations;

public static class MongoServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        var mongoSettings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
        services.AddSingleton(mongoSettings);
        var mongoConstring = Environment.GetEnvironmentVariable("MongoDbDocker") ?? mongoSettings.ConnectionString;
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConstring));
        services.AddScoped<ICollectionProvider, DefaultCollectionProvider>();

        return services;
    }
}