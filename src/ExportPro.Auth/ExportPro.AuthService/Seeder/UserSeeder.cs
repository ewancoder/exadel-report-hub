using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using Microsoft.Extensions.Logging;

namespace ExportPro.AuthService.Seeder;

public sealed class UserSeeder(
    IAuthService authService,
    IUserRepository userRepository,
    ILogger<UserSeeder> logger)
{
    public async Task SeedAsync()
    {
        logger.LogInformation("Starting user seeding process...");

        var users = GetPredefinedUsers();

        foreach (var user in users)
        {
            var existingUser = await userRepository.GetByEmailAsync(user.Email);

            if (existingUser == null)
            {
                logger.LogInformation($"Seeding user: {user.Username} ({user.Email})");
                await authService.RegisterAsync(user);
            }
            else
            {
                logger.LogInformation($"User already exists: {user.Email}");
            }
        }

        logger.LogInformation("User seeding completed successfully.");
    }

    private List<UserRegisterDto> GetPredefinedUsers() => [
                new UserRegisterDto
                {
                    Email = "G10@gmail.com",
                    Username = "GPPP",
                    Password = "G10@gmail.com",
                }
            ];
}