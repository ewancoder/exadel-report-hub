using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Middlewares;
using MediatR;
using Refit;
using ExportPro.Common.Shared.Extensions;
using Microsoft.AspNetCore.Builder;
using ExportPro.StorageService.SDK.Refit;
var builder = WebApplication.CreateBuilder(args);


//builder.Host.UseSharedSerilogAndConfiguration();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services
    .AddRefitClient<IAuth>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://authservice:8080"));

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();