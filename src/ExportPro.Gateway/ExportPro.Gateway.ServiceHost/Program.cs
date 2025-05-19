using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", false, true);
builder.Services.AddOcelot();
builder.Services.AddMvcCore();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Gateway", Version = "v1" });
});

builder.Services.AddSwaggerForOcelot(builder.Configuration);
var fronturl=Environment.GetEnvironmentVariable("FrontendUrl") ?? "https://localhost:7107";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWasm", policy =>
    {
        policy
            .WithOrigins(fronturl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();
app.UseCors("AllowWasm");
app.UseSwaggerForOcelotUI(
    opt => { },
    uiOpt =>
    {
        uiOpt.DocumentTitle = "Gateway documentation";
    }
);
app.UseSwagger();

await app.UseOcelot();

app.Run();
