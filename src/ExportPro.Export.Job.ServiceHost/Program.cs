using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Export.Job.ServiceHost.Configurations;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScheduleService();
builder.Services.AddCommonRegistrations();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
try
{
    HttpClient client = new();
    client.BaseAddress = new Uri("https://localhost:7067");
    IAuth authAPi = RestService.For<IAuth>(client);
    UserRegisterDto user = new()
    {
        Email = "G10@gmail.com",
        Username = "GPPP",
        Password = "G10@gmail.com",
    };

    var register = await authAPi.RegisterAsync(user);
}
catch
{
}

app.Run();