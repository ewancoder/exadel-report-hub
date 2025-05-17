using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Shared.IntegrationTests.Configs;
using Microsoft.Extensions.Configuration;
using Refit;

namespace ExportPro.Shared.IntegrationTests.Auth;

public static class UserLogin
{
    private static readonly IConfiguration _config = LoadingConfig.LoadConfig();
    private static readonly IAuth iauth = RestService.For<IAuth>(_config.GetSection("AuthUrl").Value!);

    public static async Task<string> Login(string email, string password)
    {
        UserLoginDto userLoginDto = new() { Email = email, Password = password };
        var user = await iauth.LoginAsync(userLoginDto);
        return user.AccessToken;
    }
}
