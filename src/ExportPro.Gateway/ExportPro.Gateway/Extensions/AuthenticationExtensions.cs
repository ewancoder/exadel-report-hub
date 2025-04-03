using System.Text;
using ExportPro.AuthService.Configuration;
using ExportPro.AuthService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace ExportPro.Gateway.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    try
                    {
                        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                        var userIdClaim = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                        var tokenVersionClaim = context.Principal?.FindFirst("tokenVersion");

                        if (userIdClaim != null && tokenVersionClaim != null)
                        {
                            var userId = new ObjectId(userIdClaim.Value);
                            var tokenVersion = int.Parse(tokenVersionClaim.Value);

                            var user = await userRepository.GetByIdAsync(userId);

                            if (user == null || user.TokenVersion != tokenVersion)
                            {
                                context.Fail("Token is no longer valid");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Fail($"Token validation failed: {ex.Message}");
                    }
                }
            };
        });

        return services;
    }
}