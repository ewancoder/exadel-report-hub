using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Auth.SDK.Models;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using MongoDB.Driver;
using Refit;

namespace ExportPro.Shared.IntegrationTests.Auth;

public static class UserActions
{
    private static readonly IMongoDbContext<User> MongoDbContext = new MongoDbContext<User>("Users");
    private static readonly IAuth iauth = RestService.For<IAuth>("http://localhost:5000");

    public static async Task RemoveUser(string name)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Username, name);
        await MongoDbContext.Collection.DeleteOneAsync(filter);
    }

    public static async Task<string> AddSuperAdmin()
    {
        UserRegisterDto userDto = new()
        {
            Username = "SuperAdminTest",
            Email = "SuperAdminTest@gmail.com",
            Password = "SuperAdminTest2@",
        };
        UserLoginDto userLoginDto = new() { Email = "SuperAdminTest@gmail.com", Password = "SuperAdminTest2@" };
        await iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.SuperAdmin);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }

    public static async Task<string> AddOwner()
    {
        UserRegisterDto userDto = new()
        {
            Username = "OwnerUserTest",
            Email = "OwnerUserTest@gmail.com",
            Password = "OwnerUserTest2@",
        };
        UserLoginDto userLoginDto = new() { Email = "OwnerUserTest@gmail.com", Password = "OwnerUserTest2@" };
        await iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.Owner);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }

    public static async Task<string> AddClientAdmin()
    {
        UserRegisterDto userDto = new()
        {
            Username = "ClientAdminTest",
            Email = "ClientAdminTest@gmail.com",
            Password = "ClientAdminTest2@",
        };
        UserLoginDto userLoginDto = new() { Email = "ClientAdminTest@gmail.com", Password = "ClientAdminTest2@" };
        await iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.ClientAdmin);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }

    public static async Task<string> AddOperator()
    {
        UserRegisterDto userDto = new()
        {
            Username = "OperatorTest",
            Email = "OperatorTest@gmail.com",
            Password = "OperatorTest2@",
        };
        UserLoginDto userLoginDto = new() { Email = "OperatorTest@gmail.com", Password = "OperatorTest2@" };
        await iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.Operator);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }
}
