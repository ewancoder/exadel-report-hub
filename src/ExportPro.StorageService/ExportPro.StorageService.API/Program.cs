using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Config;
using ExportPro.Common.Shared.Middlewares;
using MediatR;


var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSharedSerilogAndConfiguration();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

builder.Services.AddSwaggerGen();
//builder.Services.UseSwaggerUI();
var app = builder.Build();
app.UseSwagger();
//app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapOpenApi();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();