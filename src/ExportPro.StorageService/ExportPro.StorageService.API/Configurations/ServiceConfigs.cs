using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using MediatR;
using System.Reflection;

namespace ExportPro.StorageService.API.Configurations;

public static class ServiceConfigs
{
    public static void AddServiceRegistrations(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddAutoMapper(typeof(CQRS.Profiles.MappingProfile));
        services.AddScoped<ItemRepository>();
        services.AddScoped<CustomerRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IRepository<Invoice>>(
            provider => provider.GetRequiredService<IInvoiceRepository>());
        services.AddScoped<ICollectionProvider, DefaultCollectionProvider>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(AddClientFromClientDtoCommand)));
    }
}
