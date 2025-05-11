using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using Refit;

namespace ExportPro.Shared.IntegrationTests.Auth;

public static class UserLogin
{
    private static readonly IAuth iauth = RestService.For<IAuth>("http://localhost:5000");

    public static async Task<string> Login(string email, string password)
    {
        UserLoginDto userLoginDto = new() { Email = email, Password = password };
        var user = await iauth.LoginAsync(userLoginDto);
        return user.AccessToken;
    }
}
