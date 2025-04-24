using System.Reflection;
using ExportPro.Common.Shared.Behaviors;
using Microsoft.Extensions.DependencyInjection;
namespace ExportPro.StorageService.CQRS
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            return services;
        }
    }
}
