using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Config;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.Common.Shared.Refit;
using ExportPro.Export.ServiceHost.Extensions;
using ExportPro.Export.ServiceHost.Infrastructure;
using MediatR;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSharedSerilogAndConfiguration();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExportModule(builder.Configuration);

builder.Services.AddCommonRegistrations();

// Register MongoDB dependencies
builder.Services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
builder.Services.AddSingleton<ICollectionProvider, DefaultCollectionProvider>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
var baseurl = Environment.GetEnvironmentVariable("DockerForAuthUrl") ?? builder.Configuration["Refit:authUrl"];
builder
    .Services.AddRefitClient<IACLSharedApi>(
        new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer() }
    )
    .ConfigureHttpClient(
        (sp, client) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = new Uri(baseurl);
        }
    )
    .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

// MediatR behavior
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

var app = builder.Build();

// Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
