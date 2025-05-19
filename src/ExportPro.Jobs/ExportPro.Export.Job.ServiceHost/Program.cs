using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.Shared.Config;
using ExportPro.Export.Job.ServiceHost.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSharedSerilogAndConfiguration();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddScheduleService();
builder.Services.AddCommonRegistrations();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
