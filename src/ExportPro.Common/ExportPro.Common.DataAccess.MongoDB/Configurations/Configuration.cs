using ExportPro.Common.DataAccess.MongoDB.Contexts;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExportPro.Common.DataAccess.MongoDB.Configurations;

public static class Configuration
{
    public static void AddCommonRegistrations(this IServiceCollection services)
    {
        services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
        services.AddScoped<ExportProMongoContext>();
    }
}