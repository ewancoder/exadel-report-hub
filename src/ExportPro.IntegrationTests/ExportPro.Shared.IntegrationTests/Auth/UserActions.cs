using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Auth.SDK.Models;
using ExportPro.Shared.IntegrationTests.Configs;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Refit;

namespace ExportPro.Shared.IntegrationTests.Auth;

public static class UserActions
{
    private static readonly IMongoDbContext<User> MongoDbContext = new MongoDbContext<User>("Users");
    private static readonly IConfiguration _config = LoadingConfig.LoadConfig();
    private static readonly IAuth Iauth = RestService.For<IAuth>(_config.GetSection("AuthUrl").Value!);

    public static async Task RemoveUser(string name)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Username, name);
        await MongoDbContext.Collection.DeleteOneAsync(filter);
    }

    public static async Task<string> AddSuperAdmin()
    {
        UserRegisterDto userDto = new()
        {
            Username = _config.GetSection("Users:SuperAdmin:Username").Value!,
            Email = _config.GetSection("Users:SuperAdmin:Email").Value!,
            Password = _config.GetSection("Users:SuperAdmin:Password").Value!,
        };
        UserLoginDto userLoginDto = new() { Email = userDto.Email, Password = userDto.Password };
        await Iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.SuperAdmin);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await Iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }

    public static async Task<string> AddOwner()
    {
        UserRegisterDto userDto = new()
        {
            Username = _config.GetSection("Users:Owner:Username").Value!,
            Email = _config.GetSection("Users:Owner:Email").Value!,
            Password = _config.GetSection("Users:Owner:Password").Value!,
        };
        UserLoginDto userLoginDto = new() { Email = userDto.Email, Password = userDto.Password };
        await Iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.Owner);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await Iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }

    public static async Task<string> AddClientAdmin()
    {
        UserRegisterDto userDto = new()
        {
            Username = _config.GetSection("Users:ClientAdmin:Username").Value!,
            Email = _config.GetSection("Users:ClientAdmin:Email").Value!,
            Password = _config.GetSection("Users:ClientAdmin:Password").Value!,
        };
        UserLoginDto userLoginDto = new() { Email = userDto.Email, Password = userDto.Password };
        await Iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.ClientAdmin);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await Iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }

    public static async Task<string> AddOperator()
    {
        UserRegisterDto userDto = new()
        {
            Username = _config.GetSection("Users:Operator:Username").Value!,
            Email = _config.GetSection("Users:Operator:Email").Value!,
            Password = _config.GetSection("Users:Operator:Password").Value!,
        };
        UserLoginDto userLoginDto = new() { Email = userDto.Email, Password = userDto.Password };
        await Iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.Operator);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await Iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }
}
