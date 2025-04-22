using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Config;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Middlewares;
using MediatR;


var builder = WebApplication.CreateBuilder(args);

// Shared logging and config
builder.Host.UseSharedSerilogAndConfiguration();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices("ExportPro Auth Service");
builder.Services.AddCommonRegistrations();

// Register MongoDB dependencies
builder.Services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
builder.Services.AddSingleton<ICollectionProvider, DefaultCollectionProvider>();

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
