using ExportPro.Common.Shared.Config;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.Export.ServiceHost.Extensions;

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
