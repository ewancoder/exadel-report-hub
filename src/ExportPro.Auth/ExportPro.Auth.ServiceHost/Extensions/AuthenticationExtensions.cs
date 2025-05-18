using System.Security.Claims;
using System.Text;
using ExportPro.AuthService.Configuration;
using ExportPro.AuthService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace ExportPro.Auth.ServiceHost.Extensions;

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
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? string.Empty)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var userRepository = context.HttpContext.RequestServices.GetRequiredService<UserRepository>();

                    var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier);

                    if (userIdClaim == null)
                    {
                        context.Fail("Missing claims");
                        return;
                    }

                    var userId = new ObjectId(userIdClaim.Value);
                    var user = await userRepository.GetByIdAsync(userId);

                    if (user == null)
                    {
                        context.Fail("User not found");
                        return;
                    }

                    //if (!context.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                    //{
                    //    context.Fail("Refresh token missing");
                    //    return;
                    //}

                    //var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
                }
            };

        });

        return services;
    }
}