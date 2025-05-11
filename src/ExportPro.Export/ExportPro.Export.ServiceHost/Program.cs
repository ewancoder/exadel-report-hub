using ExportPro.Common.Shared.Config;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.Export.ServiceHost.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (Environment.GetEnvironmentVariable("StorageUrl") is not null)
    builder.Host.UseSharedSerilogAndConfiguration();

builder.Services.AddControllers();
builder.Services.AddExportModule(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// if (Environment.GetEnvironmentVariable("StorageUrl") is not null)
//     app.UseSerilogRequestLogging();

app.UseAuthorization();
app.MapControllers();

app.Run();
