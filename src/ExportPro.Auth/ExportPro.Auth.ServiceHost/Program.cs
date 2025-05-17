using ExportPro.Auth.CQRS.Commands;
using ExportPro.Auth.ServiceHost.Extensions;
using ExportPro.AuthService.Behaviors;
using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.DataAccess.MongoDB.Contexts;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Middlewares;
using MediatR;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices("ExportPro Auth Service");
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
builder.Services.AddScoped<ExportProMongoContext>();
builder.Services.AddScoped<ICollectionProvider, DefaultCollectionProvider>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LocalAuthorizationBehavior<,>));
builder.Services.AddScoped<IACLRepository, ACLRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IACLService, ACLService>();


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
