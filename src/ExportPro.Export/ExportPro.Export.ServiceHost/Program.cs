using ExportPro.Common.Shared.Config;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.Export.ServiceHost.Extensions;

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();
app.MapControllers();

app.Run();
